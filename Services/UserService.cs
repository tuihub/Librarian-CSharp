using Grpc.Core;
using TuiHub.Protos.User;

namespace Librarian.Services
{
    public class LibrarianSephirahService : TuiHub.Protos.Librarian.Sephirah.V1.LibrarianSephirahService.LibrarianSephirahServiceBase
    {
    }

    public class UserService : UserGrpc.UserGrpcBase
    {
        public override Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
        {
            var ret = new LoginReply();
            try
            {
                using var db = new Utils.THeamDbContext();
                string username = request.Name;
                string password = request.Password;
                var user = db.Users.Single(x => x.Name == username);
                string passwordHash = user.PasswordHash;
                bool result = Utils.PasswordHasher.VerifyHashedPassword(passwordHash, password);
                if (result == true)
                {
                    ret.UserId = user.Id;
                    ret.Token = user.Token;
                    ret.Status = TuiHub.Protos.User.Status.Success;
                }
                else
                {
                    ret.Status = TuiHub.Protos.User.Status.Failed;
                }
            }
            catch (Exception e)
            {
                ret.Status = TuiHub.Protos.User.Status.Failed;
            }
            return Task.FromResult(ret);
        }
    }
}