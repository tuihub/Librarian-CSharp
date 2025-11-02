using System.Security.Claims;
using Librarian.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Librarian.Angela.Authorization;

public class AngelaAuthorizationRequirement : IAuthorizationRequirement
{
}

public class AngelaAuthorizationHandler : AuthorizationHandler<AngelaAuthorizationRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AngelaAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        AngelaAuthorizationRequirement requirement)
    {
        // 如果已有认证（Cookie/JWT），直接放行（优先级高于 TrustedIP）
        if (context.User.Identity?.IsAuthenticated == true)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Check if request is from trusted IP
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return Task.CompletedTask;

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

        if (string.IsNullOrEmpty(remoteIpAddress)) return Task.CompletedTask;

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

        return Task.CompletedTask;
    }
}