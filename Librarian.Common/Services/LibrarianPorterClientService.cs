using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using PbBase = TuiHub.Protos.Librarian.V1;
using PbPorter = TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.Common.Services
{
    public class LibrarianPorterClientService
    {
        private readonly ILogger<LibrarianPorterClientService> _logger;
        private readonly PorterGrpcClientPool<PbPorter.LibrarianPorterService.LibrarianPorterServiceClient> _clientPool;
        private readonly PorterManagementService _porterManagementService;

        private static readonly TimeSpan ConcurrencyIncreaseThreshold = TimeSpan.FromSeconds(10);

        public LibrarianPorterClientService(
            ILogger<LibrarianPorterClientService> logger,
            PorterGrpcClientPool<PbPorter.LibrarianPorterService.LibrarianPorterServiceClient> clientPool,
            PorterManagementService porterManagementService)
        {
            _logger = logger;
            _clientPool = clientPool;
            _porterManagementService = porterManagementService;
        }

        public Task<PbPorter.Account?> GetAccountAsync(
            PbBase.FeatureRequest featureRequest,
            CancellationToken cancellationToken = default) =>
            InvokeWithPorterInnerAsync(
                featureName: "GetAccount",
                string.IsNullOrWhiteSpace(featureRequest.Region) ? null : featureRequest.Region,
                async (client, token) =>
                {
                    var request = new PbPorter.GetAccountRequest
                    {
                        Config = featureRequest,
                    };
                    var response = await client.GetAccountAsync(request, cancellationToken: token);
                    return response.Account;
                },
                cancellationToken);

        public Task<IEnumerable<PbPorter.AppInfo>?> SearchAppInfoAsync(
            PbBase.FeatureRequest featureRequest,
            CancellationToken cancellationToken = default) =>
            InvokeWithPorterInnerAsync(
                featureName: "SearchAppInfo",
                string.IsNullOrWhiteSpace(featureRequest.Region) ? null : featureRequest.Region,
                async (client, token) =>
                {
                    var request = new PbPorter.SearchAppInfoRequest
                    {
                        Config = featureRequest,
                    };
                    var response = await client.SearchAppInfoAsync(request, cancellationToken: token);
                    return response.AppInfos ?? Enumerable.Empty<PbPorter.AppInfo>();
                },
                cancellationToken);

        public Task<PbPorter.AppInfo?> GetAppInfoAsync(
            PbBase.FeatureRequest featureRequest,
            CancellationToken cancellationToken = default) =>
            InvokeWithPorterInnerAsync(
                // TODO: change FeatureRequest
                featureName: $"{Constants.PorterFeature.AppInfoSource}.{featureRequest.Id}",
                string.IsNullOrWhiteSpace(featureRequest.Region) ? null : featureRequest.Region,
                async (client, token) =>
                {
                    var request = new PbPorter.GetAppInfoRequest
                    {
                        Config = featureRequest,
                    };
                    var response = await client.GetAppInfoAsync(request, cancellationToken: token);
                    return response.AppInfo;
                },
                cancellationToken);

        public Task<PbPorter.AppInfo?> ParseRawAppInfoAsync(
            PbBase.FeatureRequest featureRequest,
            string rawDataJson,
            CancellationToken cancellationToken = default) =>
            InvokeWithPorterInnerAsync(
                featureName: "ParseRawAppInfo",
                string.IsNullOrWhiteSpace(featureRequest.Region) ? null : featureRequest.Region,
                async (client, token) =>
                {
                    var request = new PbPorter.ParseRawAppInfoRequest
                    {
                        Config = featureRequest,
                        RawDataJson = rawDataJson,
                    };
                    var response = await client.ParseRawAppInfoAsync(request, cancellationToken: token);
                    return response.AppInfo;
                },
                cancellationToken);

        private async Task<TResult?> InvokeWithPorterInnerAsync<TResult>(
            string featureName,
            string? region,
            Func<PbPorter.LibrarianPorterService.LibrarianPorterServiceClient, CancellationToken, Task<TResult?>> action,
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