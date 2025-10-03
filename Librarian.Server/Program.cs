using System.Threading.RateLimiting;
using Consul;
using IdGen.DependencyInjection;
using Librarian.Angela.Services;
using Librarian.Common;
using Librarian.Common.Configs;
using Librarian.Common.Utils;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Get Configuration
var systemConfig = builder.Configuration.GetSection("SystemConfig").Get<SystemConfig>() ??
                   throw new Exception("SystemConfig parse failed");
GlobalContext.SystemConfig = systemConfig;
var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>() ??
                throw new Exception("JwtConfig parse failed");
GlobalContext.JwtConfig = jwtConfig;
var instanceConfig = builder.Configuration.GetSection("InstanceConfig").Get<InstanceConfig>() ??
                     throw new Exception("InstanceConfig parse failed");
GlobalContext.InstanceConfig = instanceConfig;
var consulConfig = builder.Configuration.GetSection("ConsulConfig").Get<ConsulConfig>() ??
                   throw new Exception("ConsulConfig parse failed");
var massTransitConfig = builder.Configuration.GetSection("MassTransitConfig").Get<MassTransitConfig>() ??
                        throw new Exception("MassTransitConfig parse failed");
GlobalContext.MassTransitConfig = massTransitConfig;

// Add ApplicationDbContext DI
builder.Services.AddDbContext<ApplicationDbContext>();

// Add IdGen DI
builder.Services.AddIdGen(GlobalContext.SystemConfig.GeneratorId);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// Add SephirahClient for Angela to delegate services to Sephirah
builder.Services.AddGrpcClient<TuiHub.Protos.Librarian.Sephirah.V1.LibrarianSephirahService.LibrarianSephirahServiceClient>(options =>
{
    // Configure to use the same server for in-process communication
    // In a real deployment, this could point to a separate Sephirah server
    options.Address = new Uri("http://localhost:5678"); // Default Sephirah port
});

// Add Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    // AccessToken Auth (Default)
    .AddJwtBearer(options => options.GetJwtBearerOptions(GlobalContext.JwtConfig.AccessTokenAudience))
    // RefreshToken Auth
    .AddJwtBearer("RefreshToken", options => options.GetJwtBearerOptions(GlobalContext.JwtConfig.RefreshTokenAudience))
    // UploadToken Auth
    .AddJwtBearer("UploadToken", options => options.GetJwtBearerOptions(GlobalContext.JwtConfig.UploadTokenAudience))
    // DownloadToken Auth
    .AddJwtBearer("DownloadToken",
        options => options.GetJwtBearerOptions(GlobalContext.JwtConfig.DownloadTokenAudience));
builder.Services.AddAuthorization();

// Add RateLimiter
builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter("bcrypt_fixed", options =>
    {
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromSeconds(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 10;
    }));

if (consulConfig.IsEnabled)
    builder.Services.AddSingleton<IConsulClient, ConsulClient>(c => new ConsulClient(c =>
    {
        c.Address = new Uri(consulConfig.ConsulAddress);
    }));

builder.Services.AddMassTransit(x =>
{
    if (GlobalContext.MassTransitConfig.TransportType == MassTransitType.InMemory)
    {
        x.UsingInMemory((context, cfg) => { cfg.ConfigureEndpoints(context); });
    }
    else if (GlobalContext.MassTransitConfig.TransportType == MassTransitType.RabbitMq)
    {
        var rabbitMqConfig = GlobalContext.MassTransitConfig.RabbitMqConfig;

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(rabbitMqConfig.Hostname, "/", h =>
            {
                h.Username(rabbitMqConfig.Username);
                h.Password(rabbitMqConfig.Password);
            });

            cfg.ConfigureEndpoints(context);
        });

        if (!string.IsNullOrEmpty(GlobalContext.MassTransitConfig.ServiceName))
            x.SetEndpointNameFormatter(
                new KebabCaseEndpointNameFormatter(GlobalContext.MassTransitConfig.ServiceName, false));
    }
});

var app = builder.Build();

// Migrate DB
using (var scope = app.Services.CreateScope())
{
    using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<AngelaService>();

// add server reflection when env is dev
var env = app.Environment;
if (env.IsDevelopment()) app.MapGrpcReflectionService();

app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

// Enable gRPC-Web for browser support
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

// Enable Auth
app.UseAuthentication();
app.UseAuthorization();

// Enable RateLimiter
app.UseRateLimiter();

app.Run();