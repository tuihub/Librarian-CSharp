using Grpc.Core;
using Librarian.Common.Utils;
using Librarian.Porter.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.Porter.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Porter.Services
{
    public partial class PorterService : LibrarianPorterService.LibrarianPorterServiceBase
    {
        public override Task<GetPorterInformationResponse> GetPorterInformation(GetPorterInformationRequest request, ServerCallContext context)
        {
            var featureSummary = new FeatureSummary();
            featureSummary.AppInfoSources.AddRange(_globalContext.InstanceContext.AppInfoSources.Select(s => new FeatureFlag()
            {
                Id = s,
                Name = string.Empty,
                Description = string.Empty,
                ConfigJsonSchema = string.Empty,
                RequireContext = false
            }));
            var response = new GetPorterInformationResponse
            {
                BinarySummary = new TuiHub.Protos.Librarian.V1.PorterBinarySummary
                {
                    SourceCodeAddress = "https://github.com/tuihub/Librarian-CSharp",
                    BuildVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown",
                    BuildDate = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).ToISO8601String(),
                    Name = _globalContext.PorterConfig.PorterName,
                    Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown",
                    Description = string.Empty,
                },
                GlobalName = _globalContext.PorterConfig.PorterName,
                Region = _globalContext.PorterConfig.Region,
                FeatureSummary = featureSummary,
                ContextJsonSchema = string.Empty
            };
            return Task.FromResult(response);
        }
    }
}
