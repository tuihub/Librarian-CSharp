using Grpc.Core;
using Librarian.Common.Helpers;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Librarian.Angela.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Angela.Services
{
    public partial class AngelaService
    {
        [Authorize]
        public override async Task<SearchAppInfosResponse> SearchAppInfos(SearchAppInfosRequest request, ServerCallContext context)
        {
            // Verify that the user is an administrator
            UserUtil.VerifyUserAdminAndThrow(context, _dbContext);

            // Get request parameters
            var appInfos = _dbContext.AppInfos.AsQueryable();

            // Fuzzy search by name
            if (!string.IsNullOrEmpty(request.NameLike))
            {
                appInfos = appInfos.Where(a => a.Name.Contains(request.NameLike));
            }

            // Filter by source
            if (request.SourceFilter.Count > 0)
            {
                appInfos = appInfos.Where(a => request.SourceFilter.Contains(a.Source.ToString()));
            }

            // Apply paging
            appInfos = ApplyAngelaPagingRequest(appInfos, request.Paging);

            // Get results
            var appInfosList = await appInfos.ToListAsync();
            var count = await appInfos.CountAsync();

            // Build response
            var response = new SearchAppInfosResponse
            {
                Paging = new V1.PagingResponse { TotalSize = count }
            };

            // Get AppInfo objects and add to response
            foreach (var appInfo in appInfosList)
            {
                response.AppInfos.Add(ConvertToProtoAppInfo(appInfo));
            }

            return response;
        }

        private V1.AppInfo ConvertToProtoAppInfo(Common.Models.Db.AppInfo dbAppInfo)
        {
            return new V1.AppInfo
            {
                Id = new V1.InternalID { Id = dbAppInfo.Id },
                Source = ConvertToProtoAppInfoSource(dbAppInfo.Source),
                SourceAppId = dbAppInfo.SourceAppId ?? "",
                SourceUrl = dbAppInfo.SourceUrl ?? "",
                Name = dbAppInfo.Name ?? "",
                Description = dbAppInfo.Description ?? "",
                IconImageUrl = dbAppInfo.IconImageUrl ?? "",
                BackgroundImageUrl = dbAppInfo.BackgroundImageUrl ?? "",
                CoverImageUrl = dbAppInfo.CoverImageUrl ?? ""
            };
        }

        private AppInfoSource ConvertToProtoAppInfoSource(WellKnownAppInfoSource dbSource)
        {
            return dbSource switch
            {
                WellKnownAppInfoSource.Steam => AppInfoSource.Steam,
                WellKnownAppInfoSource.Vndb => AppInfoSource.Vndb,
                WellKnownAppInfoSource.Bangumi => AppInfoSource.Bangumi,
                _ => AppInfoSource.Unspecified
            };
        }
    }
}