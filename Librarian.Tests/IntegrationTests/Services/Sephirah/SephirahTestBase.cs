using Grpc.Net.Client;
using IdGen.DependencyInjection;
using Librarian.Angela.Services;
using Librarian.Common;
using Librarian.Common.Configs;
using Librarian.Common.Utils;
using Librarian.Sephirah.Server;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
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
        // TODO: fix tests
        private void EnsureServer()
        {
            if (_app == null)
            {
                var builder = WebApplication.CreateBuilder();

                StartUp.ConfigureServices(builder);
                
                // Change to dev db
                GlobalContext.SystemConfig.DbType = ApplicationDbType.MySQL;
                GlobalContext.SystemConfig.DbConnStr = Environment.GetEnvironmentVariable("DB_CONN_STR") ??
                    "server=librarian_test;port=3306;Database=librarian_test;Uid=librarian_test;Pwd=librarian_test;";
                // Apply migration
                using var dbContext = new ApplicationDbContext();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.Migrate();
                
                // Use TestServer
                builder.WebHost.UseTestServer();
                _app = builder.Build();

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
