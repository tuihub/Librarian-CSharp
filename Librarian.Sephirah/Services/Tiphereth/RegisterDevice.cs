using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<RegisterDeviceResponse> RegisterDevice(RegisterDeviceRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var deviceInfo = request.DeviceInfo;
            var deviceId = _idGenerator.CreateId();
            var device = new Common.Models.Device(deviceId, deviceInfo);
            _dbContext.Devices.Add(device);
            _dbContext.Users.Single(x => x.Id == userId).Devices.Add(device);
            _dbContext.SaveChanges();
            return Task.FromResult(new RegisterDeviceResponse
            {
                DeviceId = new InternalID { Id = deviceId }
            });
        }
    }
}
