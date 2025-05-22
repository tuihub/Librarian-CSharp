using Grpc.Core;
using Librarian.Common;
using Librarian.Common.Utils;
using LibrarianTests.IntegrationTests.Services.Sephirah;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Librarian.Tests.IntegrationTests.Services.Sephirah.Tests
{
    public partial class SephirahTest : SephirahTestBase
    {
        [Fact]
        public void Test_UsernameOrPasswordError()
        {
            // Add user
            using (var scope = App.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Users.Add(new Common.Models.Db.User
                {
                    Name = "test",
                    Password = PasswordHasher.HashPassword("test"),
                    Status = UserStatus.Active,
                });
                dbContext.SaveChanges();
            }

            // Do tests
            var client = new LibrarianSephirahService.LibrarianSephirahServiceClient(Channel);

            var usernames = new[] { "test1", "test", "test1" };
            var passwords = new[] { "test", "test1", "test1" };
            var statuses = new[] {
                new Status(StatusCode.Unauthenticated, "User not exists."),
                new Status(StatusCode.Unauthenticated, "Username and password not match."),
                new Status(StatusCode.Unauthenticated, "User not exists."),
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
