using Consul;
using IdGen.DependencyInjection;
using Librarian.Angela.Services;
using Librarian.Common;
using Librarian.Common.Configs;
using Librarian.Common.Utils;
using Librarian.Sephirah.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Sephirah.Server
{
    public static class StartUp
    {
        public static void ConfigureServices(WebApplicationBuilder builder, (ApplicationDbType dbType, string dbConnStr)? testDb)
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
            var massTransitConfig = builder.Configuration.GetSection("MassTransitConfig").Get<MassTransitConfig>() ?? throw new Exception("MassTransitConfig parse failed");
            GlobalContext.MassTransitConfig = massTransitConfig;
            var consulConfig = builder.Configuration.GetSection("ConsulConfig").Get<ConsulConfig>() ?? throw new Exception("ConsulConfig parse failed");

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
                    logger?.LogInformation("Loaded {Count} static Porter instances from configuration", systemConfig.StaticPorterInstances.Count);
                    foreach (var porter in systemConfig.StaticPorterInstances)
                    {
                        logger?.LogInformation("Static Porter: Id={Id}, Url={Url}, Tags={Tags}",
                            porter.Id, porter.Url, string.Join(",", porter.Tags));
                    }
                }

                return context;
            });

            // Add ApplicationDbContext DI
            builder.Services.AddDbContext<ApplicationDbContext>(o =>
            {
                ApplicationDbType dbType;
                string dbConnStr;
                if (testDb != null)
                {
                    dbType = testDb.Value.dbType;
                    dbConnStr = testDb.Value.dbConnStr;
                }
                else
                {
                    dbType = GlobalContext.SystemConfig.DbType;
                    dbConnStr = GlobalContext.SystemConfig.DbConnStr;
                }
                if (dbType == ApplicationDbType.SQLite)
                {
                    o.UseSqlite(dbConnStr);
                }
                else if (dbType == ApplicationDbType.MySQL)
                {
                    o.UseMySql(dbConnStr, ServerVersion.AutoDetect(dbConnStr));
                }
                else if (dbType == ApplicationDbType.PostgreSQL)
                {
                    o.UseNpgsql(dbConnStr);
                }
                else throw new ArgumentException("DbType Error.");
            });

            // Add IdGen DI
            builder.Services.AddIdGen(GlobalContext.SystemConfig.GeneratorId);

            // Add services to the container.
            builder.Services.AddGrpc().AddJsonTranscoding();
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddGrpcReflection();
            }

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
                // 注册PullMetadataService，IBusControl将通过DI自动注入
                builder.Services.AddSingleton<PullMetadataService>();
            }
            if (GlobalContext.MassTransitConfig.TransportType == MassTransitType.InMemory)
            {
                builder.Services.AddMassTransit(x =>
                {
                    // Register consumers
                    RegisterConsumers(x);

                    // Use in-memory transport
                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                });
            }
            else if (GlobalContext.MassTransitConfig.TransportType == MassTransitType.RabbitMq)
            {
                var rabbitMqConfig = GlobalContext.MassTransitConfig.RabbitMqConfig;

                builder.Services.AddMassTransit(x =>
                {
                    // Register consumers
                    RegisterConsumers(x);

                    // Use RabbitMQ transport
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(rabbitMqConfig.Hostname, "/", h =>
                        {
                            h.Username(rabbitMqConfig.Username);
                            h.Password(rabbitMqConfig.Password);
                        });

                        cfg.ConfigureEndpoints(context);
                    });

                    // Set service name
                    if (!string.IsNullOrEmpty(GlobalContext.MassTransitConfig.ServiceName))
                    {
                        x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(GlobalContext.MassTransitConfig.ServiceName, false));
                    }
                });
            }
            else
            {
                throw new NotSupportedException($"Unsupported transport type: {GlobalContext.MassTransitConfig.TransportType}");
            }
        }

        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            ConfigureServices(builder, null);
        }

        public static void Configure(WebApplication app)
        {
            // Migrate DB - commented out for testing
            //using (var scope = app.Services.CreateScope())
            //{
            //    using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //    db.Database.Migrate();
            //}

            // Configure the HTTP request pipeline.
            app.MapGrpcService<SephirahService>();
            app.MapGrpcService<Librarian.Angela.Services.AngelaService>();

            // add server reflection when env is dev
            if (app.Environment.IsDevelopment())
            {
                app.MapGrpcReflectionService();
            }

            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            // Enable gRPC-Web for browser support
            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

            // Enable Auth
            app.UseAuthentication();
            app.UseAuthorization();

            // Enable RateLimiter
            app.UseRateLimiter();

            // 启动PullMetadataService
            if (app.Services.GetService<PullMetadataService>() != null)
            {
                app.Services.GetRequiredService<PullMetadataService>().Start();

                app.Lifetime.ApplicationStopping.Register(() =>
                {
                    app.Services.GetRequiredService<PullMetadataService>().Cancel();
                });
            }
        }

        private static void RegisterConsumers(IBusRegistrationConfigurator x)
        {
        }
    }
}
