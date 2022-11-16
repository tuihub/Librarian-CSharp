using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.Sephirah.V1;
using Xunit;
using LibrarianTests.IntegrationTests.Services.Sephirah;
using Grpc.Core;

namespace LibrarianTests.Services.Sephirah.Tiphereth
{
    public partial class Tiphereth : SephirahBase
    {
        [Fact]
        public async Task Test_UsernameOrPasswordError()
        {
            var client = new LibrarianSephirahService.LibrarianSephirahServiceClient(Channel);

            var usernames = new[] { "test1", "test", "test1" };
            var passwords = new[] { "test", "test1", "test1" };
            var exception = StatusCode.PermissionDenied;

            for (int i = 0; i < usernames.Length; i++)
            {
                var u = usernames[i];
                var p = passwords[i];

                var ex = Assert.Throws<RpcException>(() =>
                {
                    var response = client.GetToken(new GetTokenRequest
                    {
                        Username = u,
                        Password = p,
                    });
                });
                Assert.Equal(exception, ex.StatusCode);
            }
        }
    }
}
