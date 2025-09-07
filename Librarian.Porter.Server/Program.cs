using Consul;
using Librarian.Common.Configs;
using Librarian.Porter.Configs;
using Librarian.Porter.Helpers;
using Librarian.Porter.Models;
using Librarian.Porter.Server.Utils;
using Librarian.Porter.Services;
using Librarian.Porter.Utils;

var builder = WebApplication.CreateBuilder(args);

// Get configuration
var globalContext = new GlobalContext();
globalContext.PorterConfig = builder.Configuration.GetSection("PorterConfig").Get<PorterConfig>() ??
                             throw new Exception("PorterConfig parse failed");
globalContext.ConsulConfig = builder.Configuration.GetSection("ConsulConfig").Get<ConsulConfig>() ??
                             throw new Exception("ConsulConfig parse failed");
builder.Services.AddSingleton(globalContext);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// Add services
if (globalContext.ConsulConfig.IsEnabled)
{
    builder.Services.AddHealthChecks();
    builder.Services.AddSingleton<IConsulClient, ConsulClient>(c => new ConsulClient(c =>
    {
        c.Address = new Uri(globalContext.ConsulConfig.ConsulAddress);
    }));
}

ServicesUtil.ConfigureThirdPartyServices(builder, globalContext, LoggerFactory.Create(b =>
{
    b.AddConsole();
    b.AddDebug();
}).CreateLogger("startup"));
builder.Services.AddSingleton<AppInfoServiceResolver>();
builder.Services.AddSingleton<AccountServiceResolver>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<PorterService>();

// add server reflection when env is dev
var env = app.Environment;
if (env.IsDevelopment()) app.MapGrpcReflectionService();

app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

if (globalContext.ConsulConfig.IsEnabled)
{
    // Set health check
    app.UseHealthChecks($"/{globalContext.ConsulConfig.HealthCheckUrl.Split('/').Last()}");

    var consulClient = app.Services.GetRequiredService<IConsulClient>();
    // Register to consul
    ConsulUtil.RegisterConsul(consulClient, globalContext);

    // Deregister from consul when app is stopped
    app.Lifetime.ApplicationStopping.Register(() => { ConsulUtil.DeregisterConsul(consulClient, globalContext); });
}

app.Run();