using System.Threading.RateLimiting;
using Consul;
using IdGen.DependencyInjection;
using Librarian.Angela.Services;
using Librarian.Common;
using Librarian.Common.Configs;
using Librarian.Common.Models;
using Librarian.Common.Utils;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcReflection();

// Add OpenAPI/Swagger only in Development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { 
            Title = "Librarian Server API", 
            Version = "v1",
            Description = "gRPC JSON Transcoding API for Librarian Server including AngelaService"
        });
    });
}

// Add services
builder.Services.AddSingleton<PullMetadataService>();

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

// Add SephirahContext DI
builder.Services.AddSingleton<SephirahContext>(provider =>
{
    var context = new SephirahContext();
    
    // Add static Porter instances from configuration
    if (systemConfig.StaticPorterInstances != null && systemConfig.StaticPorterInstances.Count > 0)
    {
        context.StaticPorterInstances = systemConfig.StaticPorterInstances;
        // Log the loaded static Porter instances
        var logger = provider.GetService<ILoggerFactory>()?.CreateLogger("StartUp");
        logger?.LogInformation("Loaded {Count} static Porter instances from configuration",
            systemConfig.StaticPorterInstances.Count);
        foreach (var porter in systemConfig.StaticPorterInstances)
            logger?.LogInformation("Static Porter: Id={Id}, Url={Url}, Tags={Tags}",
                porter.Id, porter.Url, string.Join(",", porter.Tags));
    }
    
    return context;
});

// Add ApplicationDbContext DI
builder.Services.AddDbContext<ApplicationDbContext>(o =>
{
    var dbType = GlobalContext.SystemConfig.DbType;
    var dbConnStr = GlobalContext.SystemConfig.DbConnStr;

    if (dbType == ApplicationDbType.SQLite)
        o.UseSqlite(dbConnStr);
    else if (dbType == ApplicationDbType.MySQL)
        o.UseMySql(dbConnStr, ServerVersion.AutoDetect(dbConnStr));
    else if (dbType == ApplicationDbType.PostgreSQL)
        o.UseNpgsql(dbConnStr);
    else throw new ArgumentException("DbType Error.");
});

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

// Migrate DB - commented out for demo to avoid migration issues  
// using (var scope = app.Services.CreateScope())
// {
//     using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//     db.Database.Migrate();
// }

// Configure the HTTP request pipeline.
app.MapGrpcService<AngelaService>();

// add server reflection when env is dev
var env = app.Environment;
if (env.IsDevelopment()) 
{
    app.MapGrpcReflectionService();
    
    // Configure Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Librarian Server API V1");
        c.RoutePrefix = "swagger";
    });
}

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