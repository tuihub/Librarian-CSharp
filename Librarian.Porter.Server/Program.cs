using Consul;
using Librarian.Porter.Configs;
using Librarian.Porter.Utils;
using Librarian.Porter.Server.Utils;
using Librarian.Porter.Services;

var builder = WebApplication.CreateBuilder(args);

// Get configuration
var porterConfig = builder.Configuration.GetSection("PorterConfig").Get<PorterConfig>() ?? throw new Exception("PorterConfig parse failed");
var consulConfig = builder.Configuration.GetSection("ConsulConfig").Get<ConsulConfig>() ?? throw new Exception("ConsulConfig parse failed");

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// Add services
builder.Services.AddSingleton<PorterConfig>();
builder.Services.AddSingleton<ConsulConfig>();
if (consulConfig.IsEnabled)
{
    builder.Services.AddHealthChecks();
    builder.Services.AddSingleton<IConsulClient, ConsulClient>(c => new ConsulClient(c =>
    {
        c.Address = new Uri(consulConfig.ConsulAddress);
    }));
}
ServicesUtil.ConfigureThirdPartyServices(builder, porterConfig);
builder.Services.AddSingleton<AppInfoServiceResolver>();

var app = builder.Build();

// Set configuration
app.Services.GetRequiredService<PorterConfig>().SetConfig(porterConfig);
app.Services.GetRequiredService<ConsulConfig>().SetConfig(consulConfig);

// Configure the HTTP request pipeline.
app.MapGrpcService<PorterService>();

// add server reflection when env is dev
IWebHostEnvironment env = app.Environment;
if (env.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

if (consulConfig.IsEnabled)
{
    // Set health check
    app.UseHealthChecks($"/{consulConfig.HealthCheckUrl.Split('/').Last()}");

    var consulClient = app.Services.GetRequiredService<IConsulClient>();
    // Register to consul
    Consultil.RegisterConsul(consulClient, consulConfig);

    // Deregister from consul when app is stopped
    app.Lifetime.ApplicationStopping.Register(() =>
    {
        Consultil.DeregisterConsul(consulClient);
    });
}

app.Run();
