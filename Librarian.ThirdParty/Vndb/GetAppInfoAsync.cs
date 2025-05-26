using Librarian.ThirdParty.Helpers;
using Librarian.ThirdParty.Vndb.Helpers;
using System.Text.Json;
using TuiHub.Protos.Librarian.Porter.V1;
using VndbSharp;

namespace Librarian.ThirdParty.Vndb
{
    public partial class VndbTcpApiService
    {
        public async Task<AppInfo> GetAppInfoAsync(string appIdStr, CancellationToken ct = default)
        {
            if (!uint.TryParse(appIdStr, out uint appId))
            {
                throw new ArgumentException("appIdStr must be a valid unsigned integer.");
            }

            // Create and configure client
            var client = CreateVndbClient();

            // Get visual novel by ID
            var vns = await client.GetVisualNovelAsync(VndbFilters.Id.Equals(appId), VndbSharp.Models.VndbFlags.FullVisualNovel);

            if (vns == null)
            {
                throw new Exception($"VndbSharp returned null for app {appId}");
            }
            else if (vns.Count != 1)
            {
                throw new Exception($"VndbSharp returned {vns.Count} for app {appId}");
            }

            var vn = vns.First();

            // Create short description with max 97 characters
            var shortDescription = vn.Description.Length > 97 ?
                vn.Description[..97] + "..." : vn.Description;

            return new AppInfo
            {
                Source = "vndb",
                SourceAppId = vn.Id.ToString(),
                SourceUrl = "https://vndb.org/v" + vn.Id.ToString(),
                RawDataJson = JsonSerializer.Serialize(vn, s_jso_urje),
                Details = new AppInfoDetails
                {
                    Description = vn.Description,
                    ReleaseDate = vn.Released.ToDateTime().ToUniversalTime().ToISO8601String(),
                    Developer = string.Empty,
                    Publisher = string.Empty,
                    Version = string.Empty,
                    ImageUrls = { vn.Screenshots.Select(x => x.Url) }
                },
                Name = vn.OriginalName,
                Type = AppType.Game,
                ShortDescription = shortDescription,
                IconImageUrl = string.Empty,
                BackgroundImageUrl = string.Empty,
                CoverImageUrl = vn.Image,
                Tags = { },
                NameAlternatives = { vn.Aliases }
            };
        }
    }
}