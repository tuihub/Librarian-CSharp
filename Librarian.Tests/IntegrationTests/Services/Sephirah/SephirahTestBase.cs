using Grpc.Net.Client;
using Librarian.Common;
using Librarian.Common.Configs;
using Librarian.Sephirah.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

                // Get test db
                var dbType = Environment.GetEnvironmentVariable("DB_TYPE")!.ToLower() switch
                {
                    "sqlite" => ApplicationDbType.SQLite,
                    "mysql" => ApplicationDbType.MySQL,
                    "postgresql" => ApplicationDbType.PostgreSQL,
                    _ => throw new ArgumentException($"DB_TYPE is not supported.")
                };
                var dbConnStr = Environment.GetEnvironmentVariable("DB_CONN_STR")!;
                StartUp.ConfigureServices(builder, (dbType, dbConnStr));

                // Use TestServer
                builder.WebHost.UseTestServer();
                _app = builder.Build();

                using (var scope = _app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    // Ensure create db
                    dbContext.Database.EnsureDeleted();
                    dbContext.Database.EnsureCreated();
                }

                StartUp.Configure(_app);

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
