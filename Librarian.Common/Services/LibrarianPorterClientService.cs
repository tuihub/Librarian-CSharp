using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.Common.Services
{
    public class LibrarianPorterClientService
    {
        private readonly ILogger<LibrarianPorterClientService> _logger;
        private readonly PorterGrpcClientPool<LibrarianPorterService.LibrarianPorterServiceClient> _clientPool;
        private readonly PorterManagementService _porterManagementService;

        private static readonly TimeSpan ConcurrencyIncreaseThreshold = TimeSpan.FromSeconds(10);

        public LibrarianPorterClientService(
            ILogger<LibrarianPorterClientService> logger,
            PorterGrpcClientPool<LibrarianPorterService.LibrarianPorterServiceClient> clientPool,
            PorterManagementService porterManagementService)
        {
            _logger = logger;
            _clientPool = clientPool;
            _porterManagementService = porterManagementService;
        }

        public Task<Account?> GetAccountAsync(
            string platform,
            string platformAccountId,
            string? region = null,
            CancellationToken cancellationToken = default) =>
            InvokeWithPorterInnerAsync(
                featureName: "GetAccount",
                region,
                async (client, token) =>
                {
                    var request = new GetAccountRequest
                    {
                        Platform = platform,
                        PlatformAccountId = platformAccountId
                    };
                    var response = await client.GetAccountAsync(request, cancellationToken: token);
                    return response.Account;
                },
                cancellationToken);

        public Task<IEnumerable<AppInfo>?> SearchAppInfoAsync(
            string nameLike,
            string? region = null,
            CancellationToken cancellationToken = default) =>
            InvokeWithPorterInnerAsync(
                featureName: "SearchAppInfo",
                region,
                async (client, token) =>
                {
                    var request = new SearchAppInfoRequest { NameLike = nameLike };
                    var response = await client.SearchAppInfoAsync(request, cancellationToken: token);
                    return (IEnumerable<AppInfo>)(response.AppInfos ?? Enumerable.Empty<AppInfo>());
                },
                cancellationToken);

        public Task<AppInfo?> GetAppInfoAsync(
            string source,
            string sourceAppId,
            string? region = null,
            CancellationToken cancellationToken = default) =>
            InvokeWithPorterInnerAsync(
                featureName: $"{Constants.PorterFeature.AppInfoSource}.{source}",
                region,
                async (client, token) =>
                {
                    var request = new GetAppInfoRequest
                    {
                        Source = source,
                        SourceAppId = sourceAppId
                    };
                    var response = await client.GetAppInfoAsync(request, cancellationToken: token);
                    return response.AppInfo;
                },
                cancellationToken);

        public Task<AppInfo?> ParseRawAppInfoAsync(
            string source,
            string sourceAppId,
            string rawDataJson,
            string? region = null,
            CancellationToken cancellationToken = default) =>
            InvokeWithPorterInnerAsync(
                featureName: "ParseRawAppInfo",
                region,
                async (client, token) =>
                {
                    var request = new ParseRawAppInfoRequest
                    {
                        Source = source,
                        SourceAppId = sourceAppId,
                        RawDataJson = rawDataJson
                    };
                    var response = await client.ParseRawAppInfoAsync(request, cancellationToken: token);
                    return response.AppInfo;
                },
                cancellationToken);

        private async Task<TResult?> InvokeWithPorterInnerAsync<TResult>(
            string featureName,
            string? region,
            Func<LibrarianPorterService.LibrarianPorterServiceClient, CancellationToken, Task<TResult?>> action,
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("Trying to acquire Porter for feature: {Feature}, region: {Region}", featureName, region);

            PorterInstance? porter = null;
            var stopwatch = Stopwatch.StartNew();
            bool isSuccess = false;
            TResult? result = default;

            try
            {
                porter = await _porterManagementService.AcquirePorterInstanceAsync(featureName, region, cancellationToken);
                if (porter == null)
                {
                    _logger.LogWarning("No Porter instance available for feature: {Feature} in region: {Region}", featureName, region);
                    return default;
                }

                result = await _clientPool.ExecuteWithRetryAsync(porter.Url, action, cancellationToken);
                isSuccess = true;
                return result;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.ResourceExhausted)
            {
                _logger.LogError(ex, "Resource exhausted while calling Porter. feature: {Feature}, region: {Region}", featureName, region);
                if (porter != null)
                {
                    await _porterManagementService.DecreasePorterInstanceFeatureConcurrencyAsync(porter.Id, featureName);
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling Porter. feature: {Feature}, region: {Region}", featureName, region);
                if (porter != null)
                {
                    await _porterManagementService.DecreasePorterInstanceFeatureConcurrencyAsync(porter.Id, featureName);
                }
                throw;
            }
            finally
            {
                stopwatch.Stop();
                if (porter != null)
                {
                    if (isSuccess && stopwatch.Elapsed < ConcurrencyIncreaseThreshold)
                    {
                        _porterManagementService.IncreasePorterInstanceFeatureConcurrency(porter.Id, featureName);
                    }

                    _porterManagementService.ReleasePorterInstance(porter.Id, featureName);
                    _logger.LogDebug("Released Porter instance: {PorterId} for feature: {Feature}, region: {Region}", porter.Id, featureName, region);
                }
            }
        }
    }
}