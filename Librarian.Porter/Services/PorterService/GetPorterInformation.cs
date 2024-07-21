using Grpc.Core;
using Librarian.Porter.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.Porter.Services
{
    public partial class PorterService : LibrarianPorterService.LibrarianPorterServiceBase
    {
        public override Task<GetPorterInformationResponse> GetPorterInformation(GetPorterInformationRequest request, ServerCallContext context)
        {
            var response = new GetPorterInformationResponse
            {
                Name = _porterConfig.PorterName,
                Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown",
                GlobalName = "Librarian-Porter-CSharp",
                FeatureSummary = new PorterFeatureSummary()
            };
            // TODO: update to FeatureFlag
            //if (_porterConfig.IsSteamEnabled)
            //{
            //    response.FeatureSummary.SupportedAccounts.Add(new PorterFeatureSummary.Types.Account
            //    {
            //        Platform = WellKnownAccountPlatform.Steam,
            //        AppRelationTypes = { TuiHub.Protos.Librarian.V1.AccountAppRelationType.Own }
            //    });
            //    response.FeatureSummary.SupportedAppInfoSources.Add(WellKnownAppInfoSource.Steam);
            //}
            //if (_porterConfig.IsBangumiEnabled)
            //{
            //    response.FeatureSummary.SupportedAppInfoSources.Add(WellKnownAppInfoSource.Bangumi);
            //}
            //if (_porterConfig.IsVndbEnabled)
            //{
            //    response.FeatureSummary.SupportedAppInfoSources.Add(WellKnownAppInfoSource.Vndb);
            //}
            return Task.FromResult(response);
        }
    }
}
