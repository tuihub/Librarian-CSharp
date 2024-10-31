using Consul;
using IdGen.DependencyInjection;
using Librarian.Angela.Services;
using Librarian.Common;
using Librarian.Common.Configs;
using Librarian.Common.Contracts;
using Librarian.Common.Services;
using Librarian.Common.Utils;
using Librarian.Sephirah.Configs;
using Librarian.Sephirah.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace Librarian.Sephirah.Server
{
    public static class StartUp
    {
        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            // Additional configuration is required to successfully run gRPC on macOS.
            // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

            // Get Configuration
            var systemConfig = builder.Configuration.GetSection("SystemConfig").Get<SystemConfig>() ?? throw new Exception("SystemConfig parse failed");
            GlobalContext.SystemConfig = systemConfig;
            var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>() ?? throw new Exception("JwtConfig parse failed");
            GlobalContext.JwtConfig = jwtConfig;
            var instanceConfig = builder.Configuration.GetSection("InstanceConfig").Get<InstanceConfig>() ?? throw new Exception("InstanceConfig parse failed");
            GlobalContext.InstanceConfig = instanceConfig;
            var messageQueueConfig = builder.Configuration.GetSection("messageQueueConfig").Get<MessageQueueConfig>() ?? throw new Exception("MessageQueueConfig parse failed");
            GlobalContext.MessageQueueConfig = messageQueueConfig;
            var consulConfig = builder.Configuration.GetSection("ConsulConfig").Get<ConsulConfig>() ?? throw new Exception("ConsulConfig parse failed");

            // Add SephirahContext DI
            builder.Services.AddSingleton<SephirahContext>();

            // Add ApplicationDbContext DI
            builder.Services.AddDbContext<ApplicationDbContext>();

            // Add IdGen DI
            builder.Services.AddIdGen(GlobalContext.SystemConfig.GeneratorId);

            // Add services to the container.
            builder.Services.AddGrpc();
            builder.Services.AddGrpcReflection();

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

            if (consulConfig.IsEnabled)
            {
                builder.Services.AddSingleton<IConsulClient, ConsulClient>(c => new ConsulClient(c =>
                {
                    c.Address = new Uri(consulConfig.ConsulAddress);
                }));
            }
            if (GlobalContext.MessageQueueConfig.MessageQueueType == MessageQueueType.InMemoryMq)
            {
                builder.Services.AddSingleton<IMessageQueueService, InMemoryMqService>();
            }
            else if (GlobalContext.MessageQueueConfig.MessageQueueType == MessageQueueType.RabbitMq)
            {
                var rabbitMqConfig = GlobalContext.MessageQueueConfig.RabbitMqConfig;
                var connFactory = new ConnectionFactory
                {
                    HostName = rabbitMqConfig.Hostname,
                    Port = rabbitMqConfig.Port,
                    UserName = rabbitMqConfig.Username,
                    Password = rabbitMqConfig.Password,
                    DispatchConsumersAsync = true
                };
                builder.Services.AddSingleton(connFactory.CreateConnection());
                builder.Services.AddSingleton<IMessageQueueService, RabbitMqService>();
            }
        }

        public static void Configure(WebApplication app)
        {
            // Migrate DB
            using (var scope = app.Services.CreateScope())
            {
                using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            app.MapGrpcService<SephirahService>();

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

            // Start pull metadata service
            app.Services.GetRequiredService<PullMetadataService>().Start();

            app.Lifetime.ApplicationStopping.Register(() =>
            {
                app.Services.GetRequiredService<PullMetadataService>().Cancel();
            });
        }
    }
}
