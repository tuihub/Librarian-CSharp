using Librarian.Angela.BlazorServer.Components;
using Librarian.Angela.BlazorServer.Components.Account;
using Librarian.Angela.BlazorServer.Services;
using Librarian.Angela.BlazorServer.Services.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Librarian.Angela.Services;
using Librarian.Sephirah.Services;
using Librarian.Common;
using Librarian.Common.Configs;
using Microsoft.EntityFrameworkCore;
using IdGen.DependencyInjection;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Get configuration for Angela and Sephirah services
var systemConfig = builder.Configuration.GetSection("SystemConfig").Get<SystemConfig>();
if (systemConfig != null)
{
    GlobalContext.SystemConfig = systemConfig;
    
    // Add ApplicationDbContext for Angela and Sephirah
    builder.Services.AddDbContext<ApplicationDbContext>(o =>
    {
        var dbType = systemConfig.DbType;
        var dbConnStr = systemConfig.DbConnStr;

        if (dbType == ApplicationDbType.SQLite)
            o.UseSqlite(dbConnStr);
        else if (dbType == ApplicationDbType.MySQL)
            o.UseMySql(dbConnStr, ServerVersion.AutoDetect(dbConnStr));
        else if (dbType == ApplicationDbType.PostgreSQL)
            o.UseNpgsql(dbConnStr);
        else throw new ArgumentException("DbType Error.");
    });

    // Add IdGen for unique ID generation
    builder.Services.AddIdGen(systemConfig.GeneratorId);
}

var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();
if (jwtConfig != null)
{
    GlobalContext.JwtConfig = jwtConfig;
    
    // Add JWT authentication
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => options.GetJwtBearerOptions(jwtConfig.AccessTokenAudience))
        .AddJwtBearer("RefreshToken", options => options.GetJwtBearerOptions(jwtConfig.RefreshTokenAudience));
}

// Add Angela and Sephirah services directly
builder.Services.AddScoped<AngelaService>();
builder.Services.AddScoped<SephirahService>();

// Add service for in-process communication instead of HTTP
builder.Services.AddScoped<IAngelaAuthService, AngelaAuthService>();

// Configure Sephirah API settings (keeping for backward compatibility)
builder.Services.Configure<SephirahApiConfig>(
    builder.Configuration.GetSection("SephirahApi"));

// Keep HTTP client for external calls if needed, but prioritize direct service calls
builder.Services.AddHttpClient<ISephirahAuthService, SephirahAuthService>();

// Add HTTP context accessor
builder.Services.AddHttpContextAccessor();

// Add JWT authentication state provider
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

// Keep Identity system for now (will be removed after migration is complete)
//builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();

// Add a simple redirect manager for testing
builder.Services.AddScoped<SimpleRedirectManager>();

// Configure authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies";
        options.DefaultSignInScheme = "Cookies";
    })
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// Database temporarily disabled for testing
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddSignInManager()
//    .AddDefaultTokenProviders();

//builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure request pipeline
if (app.Environment.IsDevelopment())
{
    //app.UseMigrationsEndPoint(); // Commented out for testing
}
else
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

//app.MapAdditionalIdentityEndpoints(); // Commented out for testing

app.Run();