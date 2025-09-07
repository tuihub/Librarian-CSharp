using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.ThirdParty.Contracts;

public interface IAccountService
{
    Task<Account> GetAccountAsync(string platformAccountId, CancellationToken ct = default);
}