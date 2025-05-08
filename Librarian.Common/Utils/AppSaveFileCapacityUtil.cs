using Librarian.Common.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Common.Utils
{
    public static class AppSaveFileCapacityUtil
    {
        public static CheckCapacityResult CheckCapacity(ApplicationDbContext db, long userId, long appId, long fileSizeBytes)
        {
            var user = db.Users.Single(x => x.Id == userId);
            var app = db.Apps.Single(x => x.Id == appId);
            var userAsfc = db.AppSaveFileCapacities
                .Where(x => x.UserId == userId)
                .Where(x => x.EntityType == EntityType.User)
                .Where(x => x.EntityInternalId == userId)
                .FirstOrDefault();
            if (userAsfc == null)
            {
                userAsfc = new AppSaveFileCapacity
                {
                    Count = GlobalContext.SystemConfig.UserAppSaveFileCapacityCountMax,
                    SizeBytes = GlobalContext.SystemConfig.UserAppSaveFileCapacitySizeBytesMax,
                    Strategy = Constants.Enums.AppSaveFileCapacityStrategy.Fail
                };
            }
            else
            {
                if (GlobalContext.SystemConfig.UserAppSaveFileCapacityCountMax >= 0)
                {
                    if (userAsfc.Count == null)
                    {
                        userAsfc.Count = GlobalContext.SystemConfig.UserAppSaveFileCapacityCountMax;
                    }
                    else
                    {
                        userAsfc.Count = Math.Min((long)userAsfc.Count, GlobalContext.SystemConfig.UserAppSaveFileCapacityCountMax);
                    }
                }
                if (GlobalContext.SystemConfig.UserAppSaveFileCapacitySizeBytesMax >= 0)
                {
                    if (userAsfc.SizeBytes == null)
                    {
                        userAsfc.SizeBytes = GlobalContext.SystemConfig.UserAppSaveFileCapacitySizeBytesMax;
                    }
                    else
                    {
                        userAsfc.SizeBytes = Math.Min((long)userAsfc.SizeBytes, GlobalContext.SystemConfig.UserAppSaveFileCapacitySizeBytesMax);
                    }
                }
            }
            var appAsfc = db.AppSaveFileCapacities
                .Where(x => x.UserId == userId)
                .Where(x => x.EntityType == EntityType.App)
                .Where(x => x.EntityInternalId == appId)
                .FirstOrDefault();
            if (appAsfc == null)
            {
                appAsfc = new AppSaveFileCapacity
                {
                    Count = user.AppAppSaveFileCapacityCountDefault,
                    SizeBytes = user.AppAppSaveFileCapacitySizeBytesDefault,
                    Strategy = user.AppAppSaveFileCapacityStrategyDefault
                };
            }
            var appSaveFiles = db.AppSaveFiles
                .Where(x => x.IsPinned == false)
                .Where(x => x.AppId == appId)
                .Include(x => x.FileMetadata)
                .OrderByDescending(x => x.CreatedAt);
            // check app level capacity
            CheckCapacityResult result = new(true, new List<long>());
            result.Update(CheckCapacityCount(appAsfc, appSaveFiles.ToList(), app));
            if (result.IsSuccess == false) { return result; }
            result.Update(CheckCapacitySizeBytes(appAsfc, appSaveFiles.ToList(), app, fileSizeBytes));
            if (result.IsSuccess == false) { return result; }
            // check user level capacity
            result.Update(CheckCapacityCount(userAsfc, appSaveFiles.ToList(), app));
            if (result.IsSuccess == false) { return result; }
            result.Update(CheckCapacitySizeBytes(userAsfc, appSaveFiles.ToList(), app, fileSizeBytes));
            return result;
        }

        private static CheckCapacityResult CheckCapacityCount(AppSaveFileCapacity? asfc, List<AppSaveFile> appSaveFiles, Models.Db.App app)
        {
            if (asfc == null || asfc.Count == null)
            {
                return new CheckCapacityResult(true, new List<long>());
            }
            else if (app.TotalAppSaveFileCount + 1 <= asfc.Count)
            {
                return new CheckCapacityResult(true, new List<long>());
            }
            var result = new CheckCapacityResult();
            if (appSaveFiles.Count == 0)
            {
                result.IsSuccess = false;
                result.Message = "App save file count capacity exceeded with no file can be removed.";
                return result;
            }
            switch (asfc.Strategy)
            {
                case Constants.Enums.AppSaveFileCapacityStrategy.Fail:
                    result.IsSuccess = false;
                    result.Message = "App save file count capacity exceeded with a failing strategy.";
                    break;
                case Constants.Enums.AppSaveFileCapacityStrategy.DeleteOldestOrFail:
                    if (app.TotalAppSaveFileCount <= asfc.Count)
                    {
                        result.IsSuccess = true;
                        result.AppSaveFileIdsToRemove.Add(appSaveFiles.First().Id);
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "App save file count capacity still exceeded with removing the oldest file " +
                            $"({app.TotalAppSaveFileCount} of {asfc.Count} currently used).";
                    }
                    break;
                case Constants.Enums.AppSaveFileCapacityStrategy.DeleteOldestUntilSatisfied:
                    if (app.TotalAppSaveFileCount - appSaveFiles.Count + 1 <= asfc.Count)
                    {
                        result.IsSuccess = true;
                        for (int i = 0; i < app.TotalAppSaveFileCount - asfc.Count + 1; i++)
                        {
                            result.AppSaveFileIdsToRemove.Add(appSaveFiles[i].Id);
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "App save file count capacity still exceeded with removing all files " +
                            $"({app.TotalAppSaveFileCount} of {asfc.Count} currently used).";
                    }
                    break;
                default:
                    throw new ArgumentException($"Unknown strategy: {asfc.Strategy}");
            }
            return result;
        }

        private static CheckCapacityResult CheckCapacitySizeBytes(AppSaveFileCapacity? asfc, List<AppSaveFile> appSaveFiles, Models.Db.App app, long fileSizeBytes)
        {
            if (asfc == null || asfc.SizeBytes == null)
            {
                return new CheckCapacityResult(true, new List<long>());
            }
            else if (app.TotalAppSaveFileSizeBytes + fileSizeBytes <= asfc.SizeBytes)
            {
                return new CheckCapacityResult(true, new List<long>());
            }
            var result = new CheckCapacityResult();
            if (appSaveFiles.Count == 0)
            {
                result.IsSuccess = false;
                result.Message = "App save file size capacity exceeded with no file can be removed.";
                return result;
            }
            switch (asfc.Strategy)
            {
                case Constants.Enums.AppSaveFileCapacityStrategy.Fail:
                    result.IsSuccess = false;
                    result.Message = "App save file size capacity exceeded with a failing strategy.";
                    break;
                case Constants.Enums.AppSaveFileCapacityStrategy.DeleteOldestOrFail:
                    if (app.TotalAppSaveFileSizeBytes - appSaveFiles.First().FileMetadata.SizeBytes + fileSizeBytes <= asfc.SizeBytes)
                    {
                        result.IsSuccess = true;
                        result.AppSaveFileIdsToRemove.Add(appSaveFiles.First().Id);
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "App save file size capacity still exceeded with removing the oldest file " +
                            $"({HumanizeUtil.SizeBytesToString(app.TotalAppSaveFileSizeBytes)} of {HumanizeUtil.SizeBytesToString((long)asfc.SizeBytes)} currently used).";
                    }
                    break;
                case Constants.Enums.AppSaveFileCapacityStrategy.DeleteOldestUntilSatisfied:
                    if (app.TotalAppSaveFileSizeBytes - appSaveFiles.Sum(x => x.FileMetadata.SizeBytes) + fileSizeBytes <= asfc.SizeBytes)
                    {
                        result.IsSuccess = true;
                        var cntSizeBytes = app.TotalAppSaveFileSizeBytes + fileSizeBytes;
                        var i = 0;
                        while (cntSizeBytes > asfc.SizeBytes)
                        {
                            result.AppSaveFileIdsToRemove.Add(appSaveFiles[i].Id);
                            cntSizeBytes -= appSaveFiles[i].FileMetadata.SizeBytes;
                            i++;
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "App save file size capacity still exceeded with removing all files " +
                            $"({HumanizeUtil.SizeBytesToString(app.TotalAppSaveFileSizeBytes)} of {HumanizeUtil.SizeBytesToString((long)asfc.SizeBytes)} currently used).";
                    }
                    break;
                default:
                    throw new ArgumentException($"Unknown strategy: {asfc.Strategy}");
            }
            return result;
        }
    }

    //public record struct CheckCapacityResult(bool IsSuccess, List<long> AppSaveFileIdsToRemove)
    //{
    //    public static implicit operator (bool IsSuccess, List<long> AppSaveFileIdsToRemove)(CheckCapacityResult value)
    //    {
    //        return (value.IsSuccess, value.AppSaveFileIdsToRemove);
    //    }

    //    public static implicit operator CheckCapacityResult((bool IsSuccess, List<long> AppSaveFileIdsToRemove) value)
    //    {
    //        return new CheckCapacityResult(value.IsSuccess, value.AppSaveFileIdsToRemove);
    //    }
    //}

    public class CheckCapacityResult
    {
        public bool IsSuccess { get; set; }
        public List<long> AppSaveFileIdsToRemove { get; set; } = new();
        public string? Message { get; set; }

        public CheckCapacityResult() { }
        public CheckCapacityResult(bool isSuccess, List<long> appSaveFileIdsToRemove, string? message = null)
        {
            IsSuccess = isSuccess;
            AppSaveFileIdsToRemove = appSaveFileIdsToRemove;
            Message = message;
        }
        public void Update(CheckCapacityResult ccr)
        {
            if (IsSuccess == false) { return; }
            else if (ccr.IsSuccess == false)
            {
                IsSuccess = false;
                AppSaveFileIdsToRemove = new List<long>();
                Message = ccr.Message;
            }
            else
            {
                if (ccr.AppSaveFileIdsToRemove.Count > AppSaveFileIdsToRemove.Count)
                {
                    AppSaveFileIdsToRemove = ccr.AppSaveFileIdsToRemove;
                    Message = ccr.Message;
                }
            }
        }
    }
}
