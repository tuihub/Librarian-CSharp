using Librarian.ThirdParty.Contracts;
using Librarian.ThirdParty.Helpers;
using Librarian.ThirdParty.Vndb.Helpers;
using TuiHub.Protos.Librarian.V1;
using VndbSharp;

namespace Librarian.ThirdParty.Vndb
{
    public class VndbTcpAPIService : IAppInfoService
    {
        public async Task<AppInfo> GetAppInfoAsync(string appIdStr, CancellationToken cts = default)
        {
            if (!uint.TryParse(appIdStr, out uint appId))
            {
                throw new ArgumentException("appIdStr must be a valid unsigned integer.");
            }
            var client = new VndbSharp.Vndb(useTls: true)
                                      .WithTimeout(new TimeSpan(0, 0, 30))
                                      .WithClientDetails("Librarian-CSharp", "0.1")
                                      .WithFlagsCheck(true, (method, providedFlags, invalidFlags) =>
                                      {
                                          throw new Exception($"Attempted to use method \"{method}\" with flags {providedFlags}," +
                                              $" but {invalidFlags} are not permitted on that method.");
                                      });
            var vns = await client.GetVisualNovelAsync(VndbFilters.Id.Equals(appId), VndbSharp.Models.VndbFlags.FullVisualNovel);
            if (vns == null)
            {
                throw new Exception($"VndbSharp returned null for app {appId}");
            }
            else if (vns.Count != 1)
            {
                throw new Exception($"VndbSharp returned {vns.Count} for app {appId}");
            }
            else
            {
                var vn = vns.First();
                var shortDescription = vn.Description.Length > 97 ?
                    vn.Description[..97] + "..." : vn.Description;
                return new AppInfo
                {
                    Source = "vndb",
                    SourceAppId = vn.Id.ToString(),
                    SourceUrl = "https://vndb.org/v" + vn.Id.ToString(),
                    Name = vn.OriginalName,
                    Type = AppType.Game,
                    ShortDescription = shortDescription,
                    IconImageUrl = null,
                    CoverImageUrl = vn.Image,
                    Details = new AppInfoDetails
                    {
                        Description = vn.Description,
                        ReleaseDate = vn.Released.ToDateTime().ToUniversalTime().ToISO8601String(),
                        Developer = null,
                        Publisher = null,
                        Version = null
                    }
                };
            }
        }
    }
}
