using Librarian.Porter.Configs;
using Librarian.Porter.Server.Helpers;
using Librarian.Porter.Services;

var builder = WebApplication.CreateBuilder(args);

// Get configuration
var porterConfig = builder.Configuration.GetSection("PorterConfig").Get<PorterConfig>() ?? throw new Exception("PorterConfig parse failed");

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// Add services
builder.Services.AddSingleton<PorterConfig>();
ServicesHelper.ConfigureThirdPartyServices(builder, porterConfig);

var app = builder.Build();

// Set configuration
app.Services.GetRequiredService<PorterConfig>().SetConfig(porterConfig);

// Configure the HTTP request pipeline.
app.MapGrpcService<PorterService>();

// add server reflection when env is dev
IWebHostEnvironment env = app.Environment;
if (env.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
