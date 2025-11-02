using System.Security.Claims;
using Librarian.Angela.BlazorServer.Services;
using Microsoft.AspNetCore.Authorization;

namespace Librarian.Angela.BlazorServer.Authorization;

public class AngelaAccessRequirement : IAuthorizationRequirement
{
}

public class LocalAngelaAuthorizationHandler : AuthorizationHandler<AngelaAccessRequirement>
{
    private readonly IAngelaService _angelaService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalAngelaAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IAngelaService angelaService)
    {
        _httpContextAccessor = httpContextAccessor;
        _angelaService = angelaService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        AngelaAccessRequirement requirement)
    {
        // Cookie/JWT（服务器端）已认证优先
        if (context.User.Identity?.IsAuthenticated == true)
        {
            context.Succeed(requirement);
            return;
        }

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return;

        // 兜底：调用后端接口探测 TrustedIP（不依赖本地配置）
        try
        {
            var localAdmin = await _angelaService.CheckLocalAdminAsync();
            if (localAdmin != null && localAdmin.IsLocalAdmin)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, localAdmin.Username ?? "LocalAdmin"),
                    new(ClaimTypes.Role, "Admin"),
                    new("AuthType", "TrustedIP")
                };
                var identity = new ClaimsIdentity(claims, "TrustedIP");
                httpContext.User = new ClaimsPrincipal(identity);

                context.Succeed(requirement);
            }
        }
        catch
        {
            // 忽略探测错误，由授权失败处理
        }
    }
}