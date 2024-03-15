using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Librarian.Common.Models;
using Librarian.ThirdParty.Contracts;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Steam.Models.DOTA2;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;

namespace Librarian.ThirdParty.Steam
{
    public class SteamAPIService: IAppInfoService
    {
        private readonly string _steamAPIKey;
        private readonly SteamWebInterfaceFactory _webInterfaceFactory;

        public string CurrencyCode { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;

        public SteamAPIService(string steamAPIKey)
        {
            _steamAPIKey = steamAPIKey;
            _webInterfaceFactory = new SteamWebInterfaceFactory(_steamAPIKey);
        }

        public async Task<AppInfo> GetAppInfoAsync(string appIdStr, CancellationToken cts = default)
        {
            if (!uint.TryParse(appIdStr, out uint appId))
            {
                throw new ArgumentException("appIdStr must be a valid unsigned integer.");
            }
            var webInterface = _webInterfaceFactory.CreateSteamStoreInterface(new HttpClient());
            var appInfoDetails = await webInterface.GetStoreAppDetailsAsync(appId, CurrencyCode, Language);
            if (DateTime.TryParse(appInfoDetails.ReleaseDate.Date, out DateTime appReleaseDate) == false)
                appReleaseDate = DateTime.MinValue;
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
                AppInfoDetails = new AppInfoDetails
                {
                    Description = appInfoDetails.DetailedDescription,
                    ReleaseDate = appReleaseDate,
                    Developer = string.Join(',', appInfoDetails.Developers),
                    Publisher = string.Join(',', appInfoDetails.Publishers),
                    Version = null
                }
            };
        }
    }
}
