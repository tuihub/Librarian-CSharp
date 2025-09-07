using System.Text.Json;
using Librarian.ThirdParty.Helpers;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.ThirdParty.Steam;

public partial class SteamApiService
{
    public async Task<AppInfo> GetAppInfoAsync(string appIdStr, CancellationToken ct = default)
    {
        if (!uint.TryParse(appIdStr, out var appId))
            throw new ArgumentException(
                $"{nameof(appIdStr)} must be a valid unsigned integer, got {nameof(appIdStr)}: {appIdStr}.");
        var webInterface = _webInterfaceFactory.CreateSteamStoreInterface(_httpClient);
        var appInfoDetails = await webInterface.GetStoreAppDetailsAsync(appId, CurrencyCode, Language);
        if (!DateTime.TryParse(appInfoDetails.ReleaseDate.Date, out var appReleaseDate))
            appReleaseDate = DateTime.MinValue;
        List<string> tags = [];
        tags.AddRange(appInfoDetails.Categories.Select(x => x.Description));
        tags.AddRange(appInfoDetails.Genres.Select(x => x.Description));
        return new AppInfo
        {
            Source = "steam",
            SourceAppId = appInfoDetails.SteamAppId.ToString(),
            SourceUrl = "https://store.steampowered.com/app/" + appInfoDetails.SteamAppId,
            RawDataJson = JsonSerializer.Serialize(appInfoDetails, s_jso_urje),
            Details = new AppInfoDetails
            {
                Description = appInfoDetails.DetailedDescription,
                ReleaseDate = appReleaseDate.ToUniversalTime().ToISO8601String(),
                Developer = string.Join(',', appInfoDetails.Developers),
                Publisher = string.Join(',', appInfoDetails.Publishers),
                Version = string.Empty,
                ImageUrls = { appInfoDetails.Screenshots.Select(x => x.PathFull) }
            },
            Name = appInfoDetails.Name,
            Type = AppType.Game,
            ShortDescription = appInfoDetails.ShortDescription,
            IconImageUrl = string.Empty,
            BackgroundImageUrl = appInfoDetails.Background,
            CoverImageUrl = appInfoDetails.HeaderImage,
            Tags = { tags }
        };
    }
}