using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Grpc.Core;
using Librarian.Common.Models.Db;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services;

public partial class SephirahService
{
    [Authorize]
    public override async Task<DownloadStoreAppBinaryResponse> DownloadStoreAppBinary(
        DownloadStoreAppBinaryRequest request, ServerCallContext context)
    {
        var binaryId = request.Id.Id;
        var storeAppBinary = await _dbContext.StoreAppBinaries
            .FirstOrDefaultAsync(x => x.Id == binaryId);

        if (storeAppBinary == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Store app binary not found"));

        var sentinelAppBinary = await _dbContext.Set<SentinelAppBinary>()
            .Include(x => x.SentinelLibrary)
            .ThenInclude(x => x.Sentinel)
            .FirstOrDefaultAsync(x => x.GeneratedId == storeAppBinary.SentinelGeneratedId &&
                                      x.SentinelLibrary.SentinelId == storeAppBinary.SentinelId);

        if (sentinelAppBinary == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Sentinel app binary not found"));

        var sentinel = sentinelAppBinary.SentinelLibrary.Sentinel;
        var sentinelLibrary = sentinelAppBinary.SentinelLibrary;

        var tokenRequestPayload = new
        {
            base_dir_id = sentinelLibrary.LibraryId,
            binary_dir = sentinelAppBinary.GeneratedId,
            size_bytes = sentinelAppBinary.SizeBytes
        };

        string? downloadToken = null;

        if (sentinelAppBinary.NeedToken)
            try
            {
                using var httpClient = new HttpClient();

                var librarianToken = JwtUtil.GenerateTokenForDownloadServer(context.GetInternalIdFromHeader());
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", librarianToken);

                var tokenUrl = $"{sentinel.Url.TrimEnd('/')}/{sentinel.GetTokenUrlPath.TrimStart('/')}";

                var jsonContent = JsonSerializer.Serialize(tokenRequestPayload);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(tokenUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    downloadToken = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    _logger.LogError(
                        "Failed to get download token from sentinel. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode, await response.Content.ReadAsStringAsync());
                    throw new RpcException(new Status(StatusCode.Internal,
                        $"Failed to get download token from sentinel: {response.StatusCode}"));
                }
            }
            catch (Exception ex) when (ex is not RpcException)
            {
                _logger.LogError(ex, "Error calling sentinel get_token API");
                throw new RpcException(new Status(StatusCode.Internal,
                    "Failed to get download token from sentinel"));
            }

        var downloadResponse = new DownloadStoreAppBinaryResponse
        {
            DownloadBaseUrl = $"{sentinel.Url.TrimEnd('/')}/{sentinel.DownloadFileUrlPath.TrimStart('/')}"
        };

        if (!string.IsNullOrEmpty(downloadToken)) downloadResponse.Token = downloadToken;

        foreach (var altUrl in sentinel.AltUrls)
            downloadResponse.DownloadBaseUrlAlternatives.Add(
                $"{altUrl.TrimEnd('/')}/{sentinel.DownloadFileUrlPath.TrimStart('/')}"
            );

        return downloadResponse;
    }
}