﻿using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Librarian.Common.Utils;
using System.Reflection;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        public override Task<GetServerInformationResponse> GetServerInformation(GetServerInformationRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GetServerInformationResponse
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
                CurrentTime = DateTime.UtcNow.ToTimestamp()
            });
        }
    }
}
