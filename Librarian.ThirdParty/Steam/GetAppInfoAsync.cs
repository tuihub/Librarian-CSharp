using Librarian.ThirdParty.Contracts;
using Librarian.ThirdParty.Helpers;
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
                Type = appInfoDetails.Type == "game" ? AppType.Game : AppType.Unspecified,
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
