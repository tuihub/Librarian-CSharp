using Grpc.Core;
using Librarian.Sephirah.Angela;
using Librarian.Common;
using Microsoft.AspNetCore.Http;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    public override Task<CheckLocalAdminResponse> CheckLocalAdmin(CheckLocalAdminRequest request, ServerCallContext context)
    {
        var response = new CheckLocalAdminResponse();
        
        // Check if user is authenticated
        var httpContext = context.GetHttpContext();
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var username = httpContext.User.Identity.Name ?? "Unknown";
            response.IsLocalAdmin = username == "LocalAdmin";
            response.Username = username;
            return Task.FromResult(response);
        }

        // Check if request is from trusted IP
        if (httpContext != null)
        {
            var remoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            
            // Check X-Forwarded-For header if behind a proxy
            if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    // Take the first IP in the chain (client's original IP)
                    remoteIpAddress = forwardedFor.Split(',')[0].Trim();
                }
            }
            
            if (!string.IsNullOrEmpty(remoteIpAddress))
            {
                var trustedIPs = GlobalContext.SystemConfig.AngelaTrustedIPs;
                if (trustedIPs != null && trustedIPs.Contains(remoteIpAddress))
                {
                    response.IsLocalAdmin = true;
                    response.Username = "LocalAdmin";
                    return Task.FromResult(response);
                }
            }
        }

        // Not authenticated and not from trusted IP
        response.IsLocalAdmin = false;
        response.Username = "";
        return Task.FromResult(response);
    }
}
