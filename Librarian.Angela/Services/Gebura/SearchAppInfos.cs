using Grpc.Core;
using Librarian.Common.Utils;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using InternalID = Librarian.Sephirah.Angela.InternalID;
using PagingRequest = TuiHub.Protos.Librarian.V1.PagingRequest;
using PagingResponse = Librarian.Sephirah.Angela.PagingResponse;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize(Policy = "AngelaAccess")]
    public override async Task<SearchAppInfosResponse> SearchAppInfos(SearchAppInfosRequest request,
        ServerCallContext context)
    {
        // Verify that the user is an administrator for Angela access
        UserUtil.VerifyUserAdminAndThrow(context, _dbContext);

        // Delegate to Sephirah for the actual search
        var sephirahRequest = new TuiHub.Protos.Librarian.Sephirah.V1.SearchAppInfosRequest
        {
            NameLike = request.NameLike
        };

        if (request.Paging != null)
            sephirahRequest.Paging = new PagingRequest
            {
                PageSize = request.Paging.PageSize,
                PageNum = request.Paging.PageNum
            };

        sephirahRequest.SourceFilter.AddRange(request.SourceFilter);

        // Forward the authorization header to Sephirah
        var headers = new Metadata();
        if (context.RequestHeaders.FirstOrDefault(h => h.Key == "authorization") is { } authHeader)
            headers.Add("authorization", authHeader.Value);

        try
        {
            var sephirahResponse = await _sephirahClient.SearchAppInfosAsync(sephirahRequest, headers);

            // Convert Sephirah response to Angela response format
            var response = new SearchAppInfosResponse
            {
                Paging = new PagingResponse
                {
                    TotalSize = sephirahResponse.Paging.TotalSize
                }
            };

            // Convert AppInfo from Sephirah to Angela format
            foreach (var sephirahAppInfo in sephirahResponse.AppInfos)
            {
                var appInfo = new AppInfo
                {
                    Id = new InternalID { Id = 0 }, // Sephirah AppInfo doesn't have Id, use default
                    Source = ConvertToAngelaAppInfoSourceFromString(sephirahAppInfo.Source),
                    SourceAppId = sephirahAppInfo.SourceAppId,
                    SourceUrl = sephirahAppInfo.SourceUrl,
                    Name = sephirahAppInfo.Name,
                    Description = sephirahAppInfo.Description,
                    IconImageUrl = sephirahAppInfo.IconImageUrl,
                    BackgroundImageUrl = sephirahAppInfo.BackgroundImageUrl,
                    CoverImageUrl = sephirahAppInfo.CoverImageUrl
                };
                response.AppInfos.Add(appInfo);
            }

            return response;
        }
        catch (RpcException ex)
        {
            // Re-throw Sephirah errors
            throw new RpcException(new Status(ex.StatusCode, ex.Status.Detail));
        }
    }

    private AppInfoSource ConvertToAngelaAppInfoSourceFromString(string sephirahSource)
    {
        return sephirahSource switch
        {
            "Steam" => AppInfoSource.Steam,
            "Vndb" => AppInfoSource.Vndb,
            "Bangumi" => AppInfoSource.Bangumi,
            "Internal" => AppInfoSource.Internal,
            _ => AppInfoSource.Unspecified
        };
    }
}