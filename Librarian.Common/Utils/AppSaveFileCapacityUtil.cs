using Librarian.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Common.Utils
{
    public static class AppSaveFileCapacityUtil
    {
        public static AppSaveFileCapacity GetAppSaveFileCapacity(ApplicationDbContext db, long userId, EntityType entityType, long entityId)
        {
            var asfc = db.AppSaveFileCapacities
                .Where(x => x.UserId == userId)
                .Where(x => x.EntityType == EntityType.User)
                .Where(x => x.EntityInternalId == entityId)
                .FirstOrDefault();
            if (entityType == EntityType.User)
            {
                return asfc ?? new AppSaveFileCapacity();
            }
            else
            {
                // get ids
                long appId, appInstId = -1;
                if (entityType == EntityType.App)
                {
                    appId = entityId;
                }
                else if (entityType == EntityType.AppInst)
                {
                    appInstId = entityId;
                    var appInst = db.AppInsts.Single(x => x.Id == entityId);
                    appId = appInst.AppId;
                }
                else
                {
                    throw new NotImplementedException();
                }
                // get data
                var appAsfc = db.AppSaveFileCapacities
                    .Where(x => x.UserId == userId)
                    .Where(x => x.EntityType == EntityType.App)
                    .Where(x => x.EntityInternalId == appId)
                    .FirstOrDefault();
                asfc = asfc.UpdateInfo(appAsfc);
                if (entityType == EntityType.App)
                {
                    return asfc ?? new AppSaveFileCapacity();
                }
                var appInstAsfc = db.AppSaveFileCapacities
                    .Where(x => x.UserId == userId)
                    .Where(x => x.EntityType == EntityType.AppInst)
                    .Where(x => x.EntityInternalId == appInstId)
                    .FirstOrDefault();
                asfc = asfc.UpdateInfo(appInstAsfc);
                return asfc ?? new AppSaveFileCapacity();
            }
        }

        private static AppSaveFileCapacity? UpdateInfo(this AppSaveFileCapacity? asfc, AppSaveFileCapacity? asfc2)
        {
            if (asfc == null)
            {
                return asfc2;
            }
            else
            {
                if (asfc2 == null)
                {
                    return asfc;
                }
                else
                {
                    asfc.Count = asfc2.Count ?? asfc.Count;
                    asfc.SizeBytes = asfc2.SizeBytes ?? asfc.SizeBytes;
                    asfc.Strategy = asfc2.Strategy;
                    return asfc;
                }
            }
        }
    }
}
