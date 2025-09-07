using System.Text.Json;
using Grpc.Core;
using Librarian.Common.Models.FeatureRequests;
using Microsoft.Extensions.Logging;
using NJsonSchema;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.Porter.Services;

public partial class PorterService
{
    public override async Task<GetAccountResponse> GetAccount(GetAccountRequest request, ServerCallContext context)
    {
        _logger.LogInformation("GetAccount called");

        try
        {
            var validationErrors = JsonSchema.FromType<GetAccount>().Validate(request.Config.ConfigJson);
            if (validationErrors != null && validationErrors.Count > 0)
            {
                var errorMsg =
                    $"GetAccount config validation failed: {string.Join("; ", validationErrors.Select(e => e.ToString()))}";
                _logger.LogWarning(errorMsg);
                throw new RpcException(new Status(StatusCode.InvalidArgument, errorMsg));
            }

            var config = JsonSerializer.Deserialize<GetAccount>(request.Config.ConfigJson)!;

            var accountService = _accountServiceResolver.GetService(request.Config);
            var account = await accountService.GetAccountAsync(config.AccountId, context.CancellationToken);

            return new GetAccountResponse
            {
                Account = account
            };
        }
        catch (Exception ex) when (ex is not RpcException)
        {
            _logger.LogError(ex, "Error getting account information");
            throw new RpcException(new Status(StatusCode.Internal, $"Error getting account information: {ex.Message}"));
        }
    }
}