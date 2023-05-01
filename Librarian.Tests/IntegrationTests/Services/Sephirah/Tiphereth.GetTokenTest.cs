using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.Sephirah.V1;
using Xunit;
using Grpc.Core;

namespace LibrarianTests.IntegrationTests.Services.Sephirah
{
    public partial class SephirahTest : SephirahTestBase
    {
        [Fact]
        public async Task Test_UsernameOrPasswordError()
        {
            var client = new LibrarianSephirahService.LibrarianSephirahServiceClient(Channel);

            var usernames = new[] { "test1", "test", "test1" };
            var passwords = new[] { "test", "test1", "test1" };
            var statuses = new[] {
                new Status(StatusCode.PermissionDenied, "User not exists."),
                new Status(StatusCode.PermissionDenied, "Username and password not match."),
                new Status(StatusCode.PermissionDenied, "User not exists."),
            };

            for (int i = 0; i < usernames.Length; i++)
            {
                var u = usernames[i];
                var p = passwords[i];
                var s = statuses[i];

                var ex = Assert.Throws<RpcException>(() =>
                {
                    var response = client.GetToken(new GetTokenRequest
                    {
                        Username = u,
                        Password = p,
                    });
                });
                Assert.Equal(s, ex.Status);
            }
        }
    }
}
