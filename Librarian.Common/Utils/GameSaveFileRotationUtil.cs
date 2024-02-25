namespace Librarian.Common.Utils
{
    public static class GameSaveFileRotationUtil
    {
        /// <summary>
        /// Get game savefile rotation count for app package
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userId"></param>
        /// <param name="appPackageId"></param>
        /// <returns>
        /// null for unlimited rotation count
        /// </returns>
        public static long? GetGameSaveFileRotation(ApplicationDbContext db, long userId, long appPackageId)
        {
            // AppPackage
            var ret = db.GameSaveFileRotations.SingleOrDefault(x => x.UserId == userId &&
                                                                    x.EntityInternalId == appPackageId &&
                                                                    x.ValidScope == Models.ValidScope.AppPackage)?.Count;
            if (ret != null) return ret;
            // App
            var appId = db.AppPackages.Single(x => x.Id == appPackageId).AppInfoId;
            ret = db.GameSaveFileRotations.SingleOrDefault(x => x.UserId == userId &&
                                                                x.EntityInternalId == appId &&
                                                                x.ValidScope == Models.ValidScope.App)?.Count;
            if (ret != null) return ret;
            // User
            ret = db.GameSaveFileRotations.SingleOrDefault(x => x.UserId == userId &&
                                                                x.ValidScope == Models.ValidScope.Account)?.Count;
            return ret;
        }
    }
}
