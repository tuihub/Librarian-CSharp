using Grpc.Net.Client;
using IdGen.DependencyInjection;
using Librarian.Angela.Interfaces;
using Librarian.Angela.Providers;
using Librarian.Angela.Services;
using Librarian.Common.Models;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Minio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static Google.Protobuf.Compiler.CodeGeneratorResponse.Types;

namespace LibrarianTests.IntegrationTests.Services.Sephirah
{
    public class SephirahTestBase
    {
        private WebApplication? _app;
        private HttpClient? _client;
        private GrpcChannel? _channel;
        protected GrpcChannel Channel => _channel ??= CreateChannel();
        public HttpClient Client
        {
            get
            {
                EnsureServer();
                return _client!;
            }
        }
        protected WebApplication App
        {
            get
            {
                EnsureServer();
                return _app!;
            }
        }

        // https://q.cnblogs.com/q/142563/
        private void EnsureServer()
        {
            if (_app == null)
            {
                var builder = WebApplication.CreateBuilder();
                // Get Configuration
                GlobalContext.SystemConfig = builder.Configuration.GetSection("SystemConfig").Get<SystemConfig>();
                GlobalContext.JwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();
                // Change to dev db
                GlobalContext.SystemConfig.DbType = ApplicationDbType.MySQL;
                GlobalContext.SystemConfig.DbConnStr = Environment.GetEnvironmentVariable("DB_CONN_STR") ??
                    "server=librarian_test;port=3306;Database=librarian_test;Uid=librarian_test;Pwd=librarian_test;";
                // Apply migration
                using var dbContext = new ApplicationDbContext();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
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
                // Use TestServer
                builder.WebHost.UseTestServer();
                _app = builder.Build();
                // Configure the HTTP request pipeline.
                _app.MapGrpcService<Librarian.Sephirah.Services.SephirahService>();
                // add server reflection when env is dev
                IWebHostEnvironment env = _app.Environment;
                if (env.IsDevelopment())
                {
                    _app.MapGrpcReflectionService();
                }
                //app.MapGrpcService<GreeterService>();
                //app.MapGrpcService<FileGrpcService>();
                //app.MapGrpcService<UserService>();
                _app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                // Enable Auth
                _app.UseAuthentication();
                _app.UseAuthorization();
                _app.Start();
                _client = _app.GetTestClient();
            }
        }

        private GrpcChannel CreateChannel()
        {
            return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
            {
                HttpClient = Client
            });
        }
    }
}
