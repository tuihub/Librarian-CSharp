using System.Security.Claims;
using Librarian.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static Librarian.Common.Constants.Enums;

namespace Librarian.Angela.Authorization;

public class AngelaAuthorizationRequirement : IAuthorizationRequirement
{
}

public class AngelaAuthorizationHandler : AuthorizationHandler<AngelaAuthorizationRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AngelaAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory serviceScopeFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        AngelaAuthorizationRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var internalIdStr = context.User.Claims.FirstOrDefault(c => c.Type == "internal_id")?.Value;
            if (long.TryParse(internalIdStr, out var internalId))
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == internalId);
                if (user != null && user.Type == UserType.Admin)
                {
                    context.Succeed(requirement);
                }
            }
            return;
        }

        // Check if request is from trusted IP
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return;

        // Get remote IP address, considering X-Forwarded-For for proxied requests
        var remoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString();

        // Check X-Forwarded-For header if behind a proxy
        if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
                // Take the first IP in the chain (client's original IP)
                remoteIpAddress = forwardedFor.Split(',')[0].Trim();
        }

        if (string.IsNullOrEmpty(remoteIpAddress)) return;

        var trustedIPs = GlobalContext.SystemConfig.AngelaTrustedIPs;
        if (trustedIPs != null && trustedIPs.Contains(remoteIpAddress))
        {
            // Create a LocalAdmin identity for trusted IPs
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "LocalAdmin"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("AuthType", "TrustedIP")
            };
            var identity = new ClaimsIdentity(claims, "TrustedIP");
            var principal = new ClaimsPrincipal(identity);

            // Replace the current principal with LocalAdmin
            context.Succeed(requirement);
            httpContext.User = principal;
        }
    }
}