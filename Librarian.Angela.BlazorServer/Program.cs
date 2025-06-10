using Librarian.Angela.BlazorServer.Components;
using Librarian.Angela.BlazorServer.Components.Account;
using Librarian.Angela.BlazorServer.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddQuickGridEntityFrameworkAdapter();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();


// 添加引用
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Librarian.Common;
using Librarian.Common.Configs;

// 加载JWT配置
var jwtConfig = new JwtConfig();
builder.Configuration.GetSection("JwtConfig").Bind(jwtConfig);
GlobalContext.JwtConfig = jwtConfig;

var instanceConfig = new InstanceConfig();
builder.Configuration.GetSection("InstanceConfig").Bind(instanceConfig);
GlobalContext.InstanceConfig = instanceConfig;

// 配置JWT认证
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.GetJwtBearerOptions(jwtConfig.AccessTokenAudience);
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // 允许从cookie或本地存储中读取token
            if (string.IsNullOrEmpty(context.Token))
            {
                var accessToken = context.Request.Cookies["access_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }
            }
            return Task.CompletedTask;
        }
    };
});

// 添加HttpClientFactory
builder.Services.AddHttpClient();

// 配置HTTP客户端，连接到Sephirah服务
builder.Services.AddHttpClient("SephirahAPI", client =>
{
    client.BaseAddress = new Uri("https://your-sephirah-api-url/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// 添加HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// 添加本地存储服务
builder.Services.AddProtectedBrowserStorage();

// 注册自定义认证状态提供器
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

// 注册认证服务
builder.Services.AddScoped<AuthService>();
