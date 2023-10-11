using Librarian.Angela;
using Librarian.Angela.Interfaces;
using Librarian.Angela.Providers;
using Librarian.Angela.Services;
using Librarian.Common.Models;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Minio;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Get Configuration
GlobalContext.SystemConfig = builder.Configuration.GetSection("SystemConfig").Get<SystemConfig>();
GlobalContext.JwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();

// Add ApplicationDbContext DI
builder.Services.AddDbContext<ApplicationDbContext>();

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// Add services
builder.Services.AddSingleton<PullMetadataService>();
builder.Services.AddScoped<ISteamProvider, SteamProvider>();
builder.Services.AddScoped<IVndbProvider, VndbProvider>();
builder.Services.AddScoped<IBangumiProvider, BangumiProvider>();

// Add Minio DI
builder.Services.AddMinio(c => c
    .WithEndpoint(GlobalContext.SystemConfig.MinioEndpoint)
    .WithCredentials(
        GlobalContext.SystemConfig.MinioAccessKey,
        GlobalContext.SystemConfig.MinioSecretKey)
    .WithSSL(GlobalContext.SystemConfig.MinioWithSSL));

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

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<Librarian.Sephirah.Services.SephirahService>();

// add server reflection when env is dev
IWebHostEnvironment env = app.Environment;
if (env.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

//app.MapGrpcService<GreeterService>();
//app.MapGrpcService<FileGrpcService>();
//app.MapGrpcService<UserService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

// Enable Auth
app.UseAuthentication();
app.UseAuthorization();

app.Run();
