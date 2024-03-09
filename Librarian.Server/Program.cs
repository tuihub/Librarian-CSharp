using IdGen.DependencyInjection;
using Librarian.Angela;
using Librarian.Angela.Interfaces;
using Librarian.Angela.Providers;
using Librarian.Angela.Services;
using Librarian.Common.Configs;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Minio;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Get Configuration
var systemConfig = builder.Configuration.GetSection("SystemConfig").Get<SystemConfig>() ?? throw new Exception("SystemConfig parse failed");
GlobalContext.SystemConfig = systemConfig;
var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>() ?? throw new Exception("JwtConfig parse failed");
GlobalContext.JwtConfig = jwtConfig;
var instanceConfig = builder.Configuration.GetSection("InstanceConfig").Get<InstanceConfig>() ?? throw new Exception("InstanceConfig parse failed");
GlobalContext.InstanceConfig = instanceConfig;

// Add ApplicationDbContext DI
builder.Services.AddDbContext<ApplicationDbContext>();

// Add IdGen DI
builder.Services.AddIdGen(GlobalContext.SystemConfig.GeneratorId);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// Add services
builder.Services.AddSingleton<PullMetadataService>();
builder.Services.AddScoped<ISteamProvider, SteamProvider>();
builder.Services.AddScoped<IVndbProvider, VndbProvider>();
builder.Services.AddScoped<IBangumiProvider, BangumiProvider>();

// Add Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    // AccessToken Auth (Default)
    .AddJwtBearer(options => options.GetJwtBearerOptions(GlobalContext.JwtConfig.AccessTokenAudience))
    // RefreshToken Auth
    .AddJwtBearer("RefreshToken", options => options.GetJwtBearerOptions(GlobalContext.JwtConfig.RefreshTokenAudience))
    // UploadToken Auth
    .AddJwtBearer("UploadToken", options => options.GetJwtBearerOptions(GlobalContext.JwtConfig.UploadTokenAudience))
    // DownloadToken Auth
    .AddJwtBearer("DownloadToken", options => options.GetJwtBearerOptions(GlobalContext.JwtConfig.DownloadTokenAudience));
builder.Services.AddAuthorization();

// Add RateLimiter
builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "bcrypt_fixed", options =>
    {
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromSeconds(1);
        options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 10;
    }));

var app = builder.Build();

// Migrate DB
using (var scope = app.Services.CreateScope())
{
    using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<Librarian.Sephirah.Services.SephirahService>();

// add server reflection when env is dev
IWebHostEnvironment env = app.Environment;
if (env.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

// Enable Auth
app.UseAuthentication();
app.UseAuthorization();

// Enable RateLimiter
app.UseRateLimiter();

app.Run();
