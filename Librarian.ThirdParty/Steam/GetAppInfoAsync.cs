using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Librarian.ThirdParty.Contracts;
using Librarian.ThirdParty.Helpers;
using Steam.Models.DOTA2;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.ThirdParty.Steam
{
    public partial class SteamAPIService : IAppInfoService
    {
        public async Task<AppInfo> GetAppInfoAsync(string appIdStr, CancellationToken cts = default)
        {
            if (!uint.TryParse(appIdStr, out uint appId))
            {
                throw new ArgumentException($"{nameof(appIdStr)} must be a valid unsigned integer, got {nameof(appIdStr)}: {appIdStr}.");
            }
            var webInterface = _webInterfaceFactory.CreateSteamStoreInterface(_httpClient);
            var appInfoDetails = await webInterface.GetStoreAppDetailsAsync(appId, CurrencyCode, Language);
            if (DateTime.TryParse(appInfoDetails.ReleaseDate.Date, out DateTime appReleaseDate) == false)
            {
                appReleaseDate = DateTime.MinValue;
            }
            return new AppInfo
            {
                Source = "steam",
                SourceAppId = appInfoDetails.SteamAppId.ToString(),
                SourceUrl = "https://store.steampowered.com/app/" + appInfoDetails.SteamAppId.ToString(),
                Name = appInfoDetails.Name,
                Type = appInfoDetails.Type == "game" ? TuiHub.Protos.Librarian.V1.AppType.Game : TuiHub.Protos.Librarian.V1.AppType.Unspecified,
                ShortDescription = appInfoDetails.ShortDescription,
                IconImageUrl = null,
                CoverImageUrl = appInfoDetails.HeaderImage,
                Details = new AppInfoDetails
                {
                    Description = appInfoDetails.DetailedDescription,
                    ReleaseDate = appReleaseDate.ToUniversalTime().ToISO8601String(),
                    Developer = string.Join(',', appInfoDetails.Developers),
                    Publisher = string.Join(',', appInfoDetails.Publishers),
                    Version = null
                }
            };
        }
    }
}
