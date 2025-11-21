using Grpc.Core;
using Librarian.Common.Helpers;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using InternalID = Librarian.Sephirah.Angela.InternalID;
using PagingResponse = Librarian.Sephirah.Angela.PagingResponse;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize(Policy = "AngelaAccess")]
    public override Task<SearchAppInfosResponse> SearchAppInfos(SearchAppInfosRequest request,
        ServerCallContext context)
    {
        var nameLike = request.NameLike;
        var sourceFilter = request.SourceFilter;

        var appInfos = _dbContext.AppInfos.AsQueryable();

        if (!string.IsNullOrEmpty(nameLike)) appInfos = appInfos.Where(a => a.Name.Contains(nameLike));
        if (sourceFilter.Count > 0) appInfos = appInfos.Where(a => sourceFilter.Contains(a.Source.ToString()));

        var totalSize = appInfos.Count();
        if (request.Paging != null)
        {
            var pagingRequest = s_mapper.Map<TuiHub.Protos.Librarian.V1.PagingRequest>(request.Paging);
            appInfos = appInfos.ApplyPagingRequest(pagingRequest);
        }

        var response = new SearchAppInfosResponse
        {
            Paging = new PagingResponse { TotalSize = totalSize }
        };

        foreach (var dbAppInfo in appInfos)
        {
            var flattened = dbAppInfo.Flatten();
            var appInfo = new AppInfo
            {
                Id = new InternalID { Id = 0 },
                Source = ConvertToAngelaAppInfoSourceFromString(flattened.Source.ToString()),
                SourceAppId = flattened.SourceAppId,
                SourceUrl = flattened.SourceUrl,
                Name = flattened.Name,
                Description = flattened.Description,
                IconImageUrl = flattened.IconImageUrl,
                BackgroundImageUrl = flattened.BackgroundImageUrl,
                CoverImageUrl = flattened.CoverImageUrl
            };
            response.AppInfos.Add(appInfo);
        }

        return Task.FromResult(response);
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