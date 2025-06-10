using Grpc.Core;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.Porter.Services
{
    public partial class PorterService
    {
        public override async Task<GetAccountResponse> GetAccount(GetAccountRequest request, ServerCallContext context)
        {
            _logger.LogInformation("GetAccount called for platform: {Platform}, accountId: {AccountId}",
                request.Platform, request.PlatformAccountId);

            try
            {
                var accountService = _accountServiceResolver.GetAccountService(request.Platform);
                if (accountService == null)
                {
                    _logger.LogError("No account service available for platform: {Platform}", request.Platform);
                    throw new RpcException(new Status(StatusCode.NotFound, $"No account service available for platform: {request.Platform}"));
                }

                var account = await accountService.GetAccountAsync(request.PlatformAccountId, context.CancellationToken);

                return new GetAccountResponse
                {
                    Account = account
                };
            }
            catch (Exception ex) when (ex is not RpcException)
            {
                _logger.LogError(ex, "Error getting account for platform: {Platform}, accountId: {AccountId}",
                    request.Platform, request.PlatformAccountId);
                throw new RpcException(new Status(StatusCode.Internal, $"Error getting account information: {ex.Message}"));
            }
        }
    }
}