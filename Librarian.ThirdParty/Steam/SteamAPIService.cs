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

        public async Task<App> GetAppAsync(uint appId, string currencyCode = "", string language = "")
        {
            var webInterface = _webInterfaceFactory.CreateSteamStoreInterface(new HttpClient());
            var appDetails = await webInterface.GetStoreAppDetailsAsync(appId, currencyCode, language);
            if (DateTime.TryParse(appDetails.ReleaseDate.Date, out DateTime appReleaseDate) == false)
                appReleaseDate = DateTime.MinValue;
            return new App
            {
                Source = TuiHub.Protos.Librarian.V1.AppSource.Steam,
                SourceAppId = appDetails.SteamAppId.ToString(),
                SourceUrl = "https://store.steampowered.com/app/" + appDetails.SteamAppId.ToString(),
                Name = appDetails.Name,
                Type = appDetails.Type == "game" ? TuiHub.Protos.Librarian.V1.AppType.Game : TuiHub.Protos.Librarian.V1.AppType.Unspecified,
                ShortDescription = appDetails.ShortDescription,
                IconImageUrl = null,
                HeroImageUrl = appDetails.HeaderImage,
                AppDetails = new AppDetails
                {
                    Description = appDetails.DetailedDescription,
                    ReleaseDate = appReleaseDate,
                    Developer = string.Join(',', appDetails.Developers),
                    Publisher = string.Join(',', appDetails.Publishers),
                    Version = null
                }
            };
        }
    }
}
