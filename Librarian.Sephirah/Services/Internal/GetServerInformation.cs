using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Librarian.Common;
using Librarian.Common.Utils;
using System.Reflection;
using TuiHub.Protos.Librarian.Sephirah.V1.Sephirah;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        public override Task<GetServerInformationResponse> GetServerInformation(GetServerInformationRequest request, ServerCallContext context)
        {
            var token = context.GetBearerToken();
            var valid = JwtUtil.ValidateToken(token, GlobalContext.JwtConfig.AccessTokenAudience);
            var response = new GetServerInformationResponse
            {
                ServerInformation = new ServerInformation
                {
                    ServerBinarySummary = new ServerBinarySummary
                    {
                        SourceCodeAddress = "https://github.com/tuihub/Librarian-CSharp",
                        BuildVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown",
                        BuildDate = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).ToISO8601String()
                    },
                    ProtocolSummary = new ServerProtocolSummary
                    {
                        Version = Assembly.GetAssembly(typeof(TuiHub.Protos.Librarian.V1.InternalID))?.GetName().Version?.ToString() ?? "Unknown",
                    },
                    CurrentTime = DateTime.UtcNow.ToTimestamp(),
                    ServerInstanceSummary = new ServerInstanceSummary
                    {
                        Name = GlobalContext.InstanceConfig.Name,
                        Description = GlobalContext.InstanceConfig.Description,
                        WebsiteUrl = GlobalContext.InstanceConfig.WebsiteUrl,
                        LogoUrl = GlobalContext.InstanceConfig.LogoUrl,
                        BackgroundUrl = GlobalContext.InstanceConfig.BackgroundUrl
                    },
                    // TODO: impl status
                    StatusReport = string.Empty
                }
            };
            if (valid)
            {
                // TODO: update to FeatureFlag
                //response.FeatureSummary = new ServerFeatureSummary
                //{
                //    SupportedAccountPlatforms = { },
                //    SupportedAppInfoSources = { "steam", "bangumi", "vndb" },
                //    SupportedFeedSources = { },
                //    SupportedNotifyDestinations = { }
                //};
            }
            return Task.FromResult(response);
        }
    }
}
