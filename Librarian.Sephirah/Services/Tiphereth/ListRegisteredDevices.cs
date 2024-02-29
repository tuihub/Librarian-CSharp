using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<ListRegisteredDevicesResponse> ListRegisteredDevices(ListRegisteredDevicesRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var devices = _dbContext.Users.Single(x => x.Id == userId).Devices;
            var response = new ListRegisteredDevicesResponse();
            response.Devices.AddRange(devices.Select(x => x.ToProtoDeviceInfo()));
            return Task.FromResult(response);
        }
    }
}
