using Librarian.Common.Models;
using Librarian.ThirdParty.Vndb.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VndbSharp;

namespace Librarian.ThirdParty.Vndb
{
    public class VndbTcpAPIService
    {
        public async Task<AppInfo> GetAppAsync(uint appId)
        {
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
                    Type = TuiHub.Protos.Librarian.V1.AppType.Game,
                    ShortDescription = shortDescription,
                    IconImageUrl = null,
                    BackgroundImageUrl = vn.Image,
                    AppInfoDetails = new AppInfoDetails
                    {
                        Description = vn.Description,
                        ReleaseDate = vn.Released.ToDateTime(),
                        Developer = null,
                        Publisher = null,
                        Version = null
                    }
                };
            }
        }
    }
}
