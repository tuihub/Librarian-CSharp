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
            var ret = db.GameSaveFileRotations.SingleOrDefault(x => x.EntityInternalId == appPackageId
                                                                && x.VaildScope == Models.VaildScope.AppPackage)?.Count;
            if (ret != null) return ret;
            // App
            var appId = db.AppPackages.Single(x => x.Id == appPackageId).AppId;
            ret = db.GameSaveFileRotations.SingleOrDefault(x => x.EntityInternalId == appId
                                                            && x.VaildScope == Models.VaildScope.App)?.Count;
            if (ret != null) return ret;
            // User
            ret = db.GameSaveFileRotations.SingleOrDefault(x => x.EntityInternalId == userId
                                                            && x.VaildScope == Models.VaildScope.Account)?.Count;
            return ret;
        }
    }
}
