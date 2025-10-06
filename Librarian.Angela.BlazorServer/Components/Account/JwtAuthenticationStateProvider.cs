using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Librarian.Angela.BlazorServer.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace Librarian.Angela.BlazorServer.Components.Account;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<JwtAuthenticationStateProvider> _logger;
    private readonly IAngelaService _angelaService;

    public JwtAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor,
        ILogger<JwtAuthenticationStateProvider> logger,
        IAngelaService angelaService)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _angelaService = angelaService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        // 1) 优先使用服务器端 Cookie 身份（若已登录）
        if (httpContext.User?.Identity?.IsAuthenticated == true)
        {
            return new AuthenticationState(httpContext.User);
        }

        // 2) 其次尝试 AccessToken（JWT）Cookie
        var token = httpContext.Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(token))
        {
            // 3) 最后回退：调用接口探测 TrustedIP
            try
            {
                var localAdminCheck = await _angelaService.CheckLocalAdminAsync();
                if (localAdminCheck != null && localAdminCheck.IsLocalAdmin)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, localAdminCheck.Username),
                        new Claim(ClaimTypes.Role, "Admin"),
                        new Claim("AuthType", "TrustedIP")
                    };
                    var identity = new ClaimsIdentity(claims, "TrustedIP");
                    var user = new ClaimsPrincipal(identity);
                    return new AuthenticationState(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error checking LocalAdmin status");
            }
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        try
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwt = jwtHandler.ReadJwtToken(token);

            // Check if token is expired
            if (jwt.ValidTo < DateTime.UtcNow)
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var claims = jwt.Claims.ToList();

            // Create identity with claims from JWT
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing JWT token");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public void NotifyUserAuthentication(string token)
    {
        try
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwt = jwtHandler.ReadJwtToken(token);
            var claims = jwt.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying user authentication");
        }
    }

    public void NotifyUserLogout()
    {
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}