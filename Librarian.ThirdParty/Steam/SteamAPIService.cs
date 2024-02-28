using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Librarian.Common.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Steam.Models.DOTA2;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;

namespace Librarian.ThirdParty.Steam
{
    public class SteamAPIService
    {
        private readonly string _steamAPIKey;
        private readonly SteamWebInterfaceFactory _webInterfaceFactory;
        public SteamAPIService(string steamAPIKey)
        {
            _steamAPIKey = steamAPIKey;
            _webInterfaceFactory = new SteamWebInterfaceFactory(_steamAPIKey);
        }

        public async Task<AppInfo> GetAppInfoAsync(uint appId, string currencyCode = "", string language = "")
        {
            var webInterface = _webInterfaceFactory.CreateSteamStoreInterface(new HttpClient());
            var appInfoDetails = await webInterface.GetStoreAppDetailsAsync(appId, currencyCode, language);
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
