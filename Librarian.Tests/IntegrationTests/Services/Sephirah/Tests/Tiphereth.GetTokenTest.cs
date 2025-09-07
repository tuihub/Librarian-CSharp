using Grpc.Core;
using Librarian.Common;
using Librarian.Common.Constants;
using Librarian.Common.Utils;
using LibrarianTests.IntegrationTests.Services.Sephirah;
using Microsoft.Extensions.DependencyInjection;
using TuiHub.Protos.Librarian.Sephirah.V1;
using Xunit;
using Assert = Xunit.Assert;
using User = Librarian.Common.Models.Db.User;

namespace Librarian.Tests.IntegrationTests.Services.Sephirah.Tests;

public class SephirahTest : SephirahTestBase
{
    [Fact]
    public void Test_UsernameOrPasswordError()
    {
        // Add user
        using (var scope = App.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Users.Add(new User
            {
                Name = "test",
                Password = PasswordHasher.HashPassword("test"),
                Status = Enums.UserStatus.Active
            });
            dbContext.SaveChanges();
        }

        // Do tests
        var client = new LibrarianSephirahService.LibrarianSephirahServiceClient(Channel);

        var usernames = new[] { "test1", "test", "test1" };
        var passwords = new[] { "test", "test1", "test1" };
        var statuses = new[]
        {
            new Status(StatusCode.Unauthenticated, "User not exists."),
            new Status(StatusCode.Unauthenticated, "Username and password not match."),
            new Status(StatusCode.Unauthenticated, "User not exists.")
        };

        for (var i = 0; i < usernames.Length; i++)
        {
            var u = usernames[i];
            var p = passwords[i];
            var s = statuses[i];

            var ex = Assert.Throws<RpcException>(() =>
            {
                var response = client.GetToken(new GetTokenRequest
                {
                    Username = u,
                    Password = p
                });
            });
            Assert.Equal(s, ex.Status);
        }
    }
}