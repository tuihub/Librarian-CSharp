using Librarian.Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Common.Utils
{
    public static class AppSaveFileCapacityUtil
    {
        public static CheckCapacityResult CheckCapacity(ApplicationDbContext db, long userId, long appId, long fileSizeBytes)
        {
            var userAsfc = db.AppSaveFileCapacities
                .Where(x => x.UserId == userId)
                .Where(x => x.EntityType == EntityType.User)
                .Where(x => x.EntityInternalId == userId)
                .FirstOrDefault();
            var appAsfc = db.AppSaveFileCapacities
                .Where(x => x.UserId == userId)
                .Where(x => x.EntityType == EntityType.App)
                .Where(x => x.EntityInternalId == appId)
                .FirstOrDefault();
            var user = db.Users.Single(x => x.Id == userId);
            var app = db.Apps.Single(x => x.Id == appId);
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

        private static void Update(ref this CheckCapacityResult result, CheckCapacityResult newResult)
        {
            if (result.IsSuccess == false) { return; }
            else if (newResult.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.AppSaveFileIdsToRemove = new List<long>();
            }
            else
            {
                if (newResult.AppSaveFileIdsToRemove.Count > result.AppSaveFileIdsToRemove.Count)
                {
                    result.AppSaveFileIdsToRemove = newResult.AppSaveFileIdsToRemove;
                }
            }
        }

        private static CheckCapacityResult CheckCapacityCount(AppSaveFileCapacity? asfc, List<AppSaveFile> appSaveFiles, Models.App app)
        {
            if (asfc == null || asfc.Count == null)
            {
                return (true, new List<long>());
            }
            else if (app.TotalAppSaveFileCount + 1 <= asfc.Count)
            {
                return (true, new List<long>());
            }
            bool result;
            List<long> appSaveFileIdsToRemove = new();
            switch (asfc.Strategy)
            {
                case AppSaveFileCapacityStrategy.Fail:
                    result = false;
                    break;
                case AppSaveFileCapacityStrategy.DeleteOldest:
                    if (app.TotalAppSaveFileCount <= asfc.Count)
                    {
                        result = true;
                        appSaveFileIdsToRemove.Add(appSaveFiles.First().Id);
                    }
                    else
                    {
                        result = false;
                    }
                    break;
                case AppSaveFileCapacityStrategy.DeleteOldestUntilSatisfied:
                    if (app.TotalAppSaveFileCount - appSaveFiles.Count + 1 <= asfc.Count)
                    {
                        result = true;
                        for (int i = 0; i < app.TotalAppSaveFileCount - asfc.Count + 1; i++)
                        {
                            appSaveFileIdsToRemove.Add(appSaveFiles[i].Id);
                        }
                    }
                    else
                    {
                        result = false;
                    }
                    break;
                default:
                    throw new ArgumentException($"Unknown strategy: {asfc.Strategy}");
            }
            return new CheckCapacityResult(result, appSaveFileIdsToRemove);
        }

        private static CheckCapacityResult CheckCapacitySizeBytes(AppSaveFileCapacity? asfc, List<AppSaveFile> appSaveFiles, Models.App app, long fileSizeBytes)
        {
            if (asfc == null || asfc.SizeBytes == null)
            {
                return (true, new List<long>());
            }
            else if (app.TotalAppSaveFileSizeBytes + fileSizeBytes <= asfc.SizeBytes)
            {
                return (true, new List<long>());
            }
            bool result;
            List<long> appSaveFileIdsToRemove = new();
            switch (asfc.Strategy)
            {
                case AppSaveFileCapacityStrategy.Fail:
                    result = false;
                    break;
                case AppSaveFileCapacityStrategy.DeleteOldest:
                    if (appSaveFiles.Count == 0)
                    {
                        result = false;
                    }
                    else if (app.TotalAppSaveFileSizeBytes - appSaveFiles.First().FileMetadata.SizeBytes + fileSizeBytes <= asfc.SizeBytes)
                    {
                        result = true;
                        appSaveFileIdsToRemove.Add(appSaveFiles.First().Id);
                    }
                    else
                    {
                        result = false;
                    }
                    break;
                case AppSaveFileCapacityStrategy.DeleteOldestUntilSatisfied:
                    if (app.TotalAppSaveFileSizeBytes - appSaveFiles.Sum(x => x.FileMetadata.SizeBytes) + fileSizeBytes <= asfc.SizeBytes)
                    {
                        result = true;
                        var cntSizeBytes = app.TotalAppSaveFileSizeBytes + fileSizeBytes;
                        var i = 0;
                        while (cntSizeBytes > asfc.SizeBytes)
                        {
                            appSaveFileIdsToRemove.Add(appSaveFiles[i].Id);
                            cntSizeBytes -= appSaveFiles[i].FileMetadata.SizeBytes;
                            i++;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                    break;
                default:
                    throw new ArgumentException($"Unknown strategy: {asfc.Strategy}");
            }
            return new CheckCapacityResult(result, appSaveFileIdsToRemove);
        }
    }

    public record struct CheckCapacityResult(bool IsSuccess, List<long> AppSaveFileIdsToRemove)
    {
        public static implicit operator (bool IsSuccess, List<long> AppSaveFileIdsToRemove)(CheckCapacityResult value)
        {
            return (value.IsSuccess, value.AppSaveFileIdsToRemove);
        }

        public static implicit operator CheckCapacityResult((bool IsSuccess, List<long> AppSaveFileIdsToRemove) value)
        {
            return new CheckCapacityResult(value.IsSuccess, value.AppSaveFileIdsToRemove);
        }
    }
}
