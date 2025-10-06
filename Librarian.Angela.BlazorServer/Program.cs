using Librarian.Angela.BlazorServer.Components;
using Librarian.Angela.BlazorServer.Components.Account;
using Librarian.Angela.BlazorServer.Services;
using Librarian.Angela.BlazorServer.Services.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authorization;
using Librarian.Angela.BlazorServer.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure API settings for both Sephirah and Angela
builder.Services.Configure<SephirahApiConfig>(
    builder.Configuration.GetSection("SephirahApi"));
builder.Services.Configure<AngelaApiConfig>(
    builder.Configuration.GetSection("AngelaApi"));

// Add HttpClient for both APIs
builder.Services.AddHttpClient<ISephirahAuthService, SephirahAuthService>();
builder.Services.AddHttpClient<IAngelaService, AngelaService>();

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

// 注册 AngelaAccess 策略，并设为默认策略：
// 规则：Cookie/现有身份优先，TrustedIP 作为兜底
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .AddRequirements(new AngelaAccessRequirement())
        .Build();
    options.AddPolicy("AngelaAccess", policy =>
    {
        policy.Requirements.Add(new AngelaAccessRequirement());
    });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IAuthorizationHandler, LocalAngelaAuthorizationHandler>();

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

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

//app.MapAdditionalIdentityEndpoints(); // Commented out for testing

app.Run();