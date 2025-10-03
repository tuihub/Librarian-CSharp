using System.Security.Claims;
using Grpc.Core;
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
        // If user is already authenticated, allow
        if (context.User.Identity?.IsAuthenticated == true)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Check if request is from trusted IP
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return Task.CompletedTask;
        }

        var remoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrEmpty(remoteIpAddress))
        {
            return Task.CompletedTask;
        }

        var trustedIPs = GlobalContext.SystemConfig.AngelaTrustedIPs;
        if (trustedIPs != null && trustedIPs.Contains(remoteIpAddress))
        {
            // Create a LocalAdmin identity for trusted IPs
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "LocalAdmin"),
                new Claim(ClaimTypes.Role, "Admin")
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
