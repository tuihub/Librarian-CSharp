using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;
using App = Librarian.Common.Models.Db.App;

namespace Librarian.Sephirah.Services;

public partial class SephirahService
{
    // TODO: impl DeviceId
    /// <summary>
    ///     Acquire a store app for the current user
    /// </summary>
    [Authorize]
    public override async Task<AcquireStoreAppResponse> AcquireStoreApp(AcquireStoreAppRequest request,
        ServerCallContext context)
    {
        var userId = context.GetInternalIdFromHeader();

        // Find the store app
        var storeApp = await _dbContext.StoreApps
            .FirstOrDefaultAsync(x => x.Id == request.StoreAppId.Id);

        if (storeApp == null) throw new RpcException(new Status(StatusCode.NotFound, "Store app not found"));

        // Check if user already acquired this app
        var existingApp = await _dbContext.Apps
            .FirstOrDefaultAsync(x => x.UserId == userId && x.BoundStoreAppId == storeApp.Id);

        if (existingApp != null)
            // User already has this app
            return new AcquireStoreAppResponse
            {
                AppId = new InternalID { Id = existingApp.Id }
            };

        // Get user's device
        var user = await _dbContext.Users.Include(u => u.Devices).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

        // Get or create a device for the user
        //long deviceId;
        //if (user.Devices.Any())
        //{
        //    deviceId = user.Devices.OrderByDescending(d => d.UpdatedAt).First().Id;
        //}
        //else
        //{
        //    // Create a new device if none exists
        //    deviceId = _idGenerator.CreateId();
        //    var newDevice = new Common.Models.Db.Device
        //    {
        //        Id = deviceId,
        //        DeviceName = "Default Device",
        //        SystemType = Common.Constants.Enums.SystemType.Unspecified,
        //        CreatedAt = DateTime.UtcNow,
        //        UpdatedAt = DateTime.UtcNow
        //    };

        //    user.Devices.Add(newDevice);
        //    await _dbContext.SaveChangesAsync();
        //}

        // Create a new app for the user based on the store app
        var appId = _idGenerator.CreateId();
        var app = new App
        {
            Id = appId,
            UserId = userId,
            Name = storeApp.Name,
            Type = storeApp.Type,
            Description = storeApp.Description,
            IconImageId = storeApp.IconImageId,
            BackgroundImageId = storeApp.BackgroundImageId,
            CoverImageId = storeApp.CoverImageId,
            Tags = storeApp.Tags,
            AltNames = storeApp.AltNames,
            Developer = storeApp.Developer,
            Publisher = storeApp.Publisher,
            AppSources = storeApp.AppSources,
            IsPublic = false,
            //CreatorDeviceId = deviceId,
            BoundStoreAppId = storeApp.Id,
            StopStoreManage = false,
            RevisedVersion = 1,
            RevisedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Apps.Add(app);
        await _dbContext.SaveChangesAsync();

        return new AcquireStoreAppResponse
        {
            AppId = new InternalID { Id = appId }
        };
    }
}