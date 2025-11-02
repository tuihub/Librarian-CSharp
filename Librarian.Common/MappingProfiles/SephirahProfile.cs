using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Librarian.Common.Constants;
using Librarian.Common.Converters;
using Librarian.Common.Models.Db;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;
using Account = Librarian.Common.Models.Db.Account;
using App = TuiHub.Protos.Librarian.Sephirah.V1.App;
using AppCategory = Librarian.Common.Models.Db.AppCategory;
using AppInfo = TuiHub.Protos.Librarian.Sephirah.V1.AppInfo;
using AppRunTime = Librarian.Common.Models.Db.AppRunTime;
using Device = Librarian.Common.Models.Db.Device;
using Enum = System.Enum;
using FileMetadata = Librarian.Common.Models.Db.FileMetadata;

namespace Librarian.Common.MappingProfiles;

public class SephirahProfile : Profile
{
    public SephirahProfile()
    {
        CreateMap<string, WellKnowns.AppInfoSource>()
            .ConvertUsing(s => Enum.Parse<WellKnowns.AppInfoSource>(s));

        CreateMap<AppType, Enums.AppType>()
            .ConvertUsingEnumMapping(opt => opt.MapByName());

        CreateMap<Enums.AppType, AppType>()
            .ConvertUsingEnumMapping(opt => opt.MapByName());

        // SystemType enum mapping (DB <-> Proto)
        CreateMap<Enums.SystemType, SystemType>()
            .ConvertUsingEnumMapping(opt => opt.MapByName());

        // FileType enum mapping (DB -> Proto)
        CreateMap<Enums.FileType, FileType>()
            .ConvertUsingEnumMapping(opt => opt.MapByName());

        // AppInfo: Protobuf -> DB
        CreateMap<AppInfo, Models.Db.AppInfo>()
            .ForMember(dest => dest.IconImageId, opt => opt.MapFrom(src => src.IconImageId.Id))
            .ForMember(dest => dest.BackgroundImageId, opt => opt.MapFrom(src => src.BackgroundImageId.Id))
            .ForMember(dest => dest.CoverImageId, opt => opt.MapFrom(src => src.CoverImageId.Id))
            .ForMember(dest => dest.AltNames, opt => opt.MapFrom(src => src.NameAlternatives.ToList()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.ToList()))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AppBinaries, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.AccountId, opt => opt.Ignore())
            .ForMember(dest => dest.Account, opt => opt.Ignore());

        // AppInfo: DB -> Protobuf
        CreateMap<Models.Db.AppInfo, AppInfo>()
            .ForMember(dest => dest.IconImageId, opt => opt.MapFrom(src => new InternalID { Id = src.IconImageId }))
            .ForMember(dest => dest.BackgroundImageId,
                opt => opt.MapFrom(src => new InternalID { Id = src.BackgroundImageId }))
            .ForMember(dest => dest.CoverImageId, opt => opt.MapFrom(src => new InternalID { Id = src.CoverImageId }))
            .ForMember(dest => dest.NameAlternatives, opt => opt.MapFrom(src => src.AltNames))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));

        // App: Protobuf -> DB
        CreateMap<App, Models.Db.App>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Id))
            .ForMember(dest => dest.RevisedVersion, opt => opt.MapFrom(src => src.VersionNumber))
            .ForMember(dest => dest.RevisedAt, opt => opt.MapFrom(src => src.VersionDate.ToDateTime()))
            .ForMember(dest => dest.CreatorDeviceId, opt => opt.MapFrom(src => src.CreatorDeviceId.Id))
            .ForMember(dest => dest.AppSources, opt => opt.MapFrom(src => src.AppSources.ToDictionary(kv =>
                kv.Key.ToEnum<WellKnowns.AppInfoSource>(), kv => kv.Value)))
            .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.Public))
            .ForMember(dest => dest.BoundStoreAppId, opt => opt.MapFrom(src => src.BoundStoreAppId.Id))
            .ForMember(dest => dest.IconImageId, opt => opt.MapFrom(src => src.IconImageId.Id))
            .ForMember(dest => dest.BackgroundImageId, opt => opt.MapFrom(src => src.BackgroundImageId.Id))
            .ForMember(dest => dest.CoverImageId, opt => opt.MapFrom(src => src.CoverImageId.Id))
            .ForMember(dest => dest.AltNames, opt => opt.MapFrom(src => src.NameAlternatives.ToList()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.ToList()))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.TotalRunTime, opt => opt.Ignore())
            .ForMember(dest => dest.TotalAppSaveFileCount, opt => opt.Ignore())
            .ForMember(dest => dest.TotalAppSaveFileSizeBytes, opt => opt.Ignore())
            .ForMember(dest => dest.AppRunTimes, opt => opt.Ignore())
            .ForMember(dest => dest.AppSaveFiles, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.AppInfoId, opt => opt.Ignore())
            .ForMember(dest => dest.AppInfo, opt => opt.Ignore())
            .ForMember(dest => dest.AppCategories, opt => opt.Ignore());

        // App: DB -> Protobuf
        CreateMap<Models.Db.App, App>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new InternalID { Id = src.Id }))
            .ForMember(dest => dest.VersionNumber, opt => opt.MapFrom(src => src.RevisedVersion))
            .ForMember(dest => dest.VersionDate,
                opt => opt.MapFrom(src => Timestamp.FromDateTime(src.RevisedAt.ToUniversalTime())))
            .ForMember(dest => dest.CreatorDeviceId,
                opt => opt.MapFrom(src => new InternalID { Id = src.CreatorDeviceId }))
            .ForMember(dest => dest.AppSources,
                opt => opt.MapFrom(src => src.AppSources.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value)))
            .ForMember(dest => dest.Public, opt => opt.MapFrom(src => src.IsPublic))
            .ForMember(dest => dest.BoundStoreAppId,
                opt => opt.MapFrom(src => new InternalID { Id = src.BoundStoreAppId }))
            .ForMember(dest => dest.IconImageId, opt => opt.MapFrom(src => new InternalID { Id = src.IconImageId }))
            .ForMember(dest => dest.BackgroundImageId,
                opt => opt.MapFrom(src => new InternalID { Id = src.BackgroundImageId }))
            .ForMember(dest => dest.CoverImageId, opt => opt.MapFrom(src => new InternalID { Id = src.CoverImageId }))
            .ForMember(dest => dest.NameAlternatives, opt => opt.MapFrom(src => src.AltNames))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));

        CreateMap<AppCategory, TuiHub.Protos.Librarian.Sephirah.V1.AppCategory>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new InternalID { Id = src.Id }))
            .ForMember(dest => dest.VersionNumber, opt => opt.MapFrom(src => 0UL))
            .ForMember(dest => dest.VersionDate, opt => opt.MapFrom(src =>
                src.UpdatedAt.HasValue
                    ? Timestamp.FromDateTime(src.UpdatedAt.Value.ToUniversalTime())
                    : Timestamp.FromDateTime(src.CreatedAt.ToUniversalTime())))
            .ForMember(dest => dest.AppIds,
                opt => opt.MapFrom(src => src.Apps.Select(a => new InternalID { Id = a.Id })));

        // Device: DB -> Protobuf
        CreateMap<Device, TuiHub.Protos.Librarian.Sephirah.V1.Device>()
            .ForMember(dest => dest.DeviceId, opt => opt.MapFrom(src => new InternalID { Id = src.Id }));

        // Session -> UserSession: DB -> Protobuf
        CreateMap<Session, UserSession>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new InternalID { Id = src.Id }))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => new InternalID { Id = src.UserId }))
            .ForMember(dest => dest.DeviceInfo, opt => opt.MapFrom(src => src.Device))
            .ForMember(dest => dest.CreateTime,
                opt => opt.MapFrom(src => Timestamp.FromDateTime(src.CreatedAt.ToUniversalTime())))
            .ForMember(dest => dest.ExpireTime,
                opt => opt.MapFrom(src => Timestamp.FromDateTime(src.ExpiredAt.ToUniversalTime())));

        // Account: DB -> Protobuf
        CreateMap<Account, TuiHub.Protos.Librarian.Sephirah.V1.Account>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new InternalID { Id = src.Id }))
            .ForMember(dest => dest.Platform, opt => opt.MapFrom(src => src.Platform ?? string.Empty))
            .ForMember(dest => dest.PlatformAccountId, opt => opt.MapFrom(src => src.PlatformAccountId ?? string.Empty))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty))
            .ForMember(dest => dest.ProfileUrl, opt => opt.MapFrom(src => src.ProfileUrl ?? string.Empty))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl ?? string.Empty))
            .ForMember(dest => dest.LatestUpdateTime,
                opt => opt.MapFrom(src => Timestamp.FromDateTime((src.UpdatedAt ?? src.CreatedAt).ToUniversalTime())));

        // AppRunTime: DB -> Protobuf
        CreateMap<AppRunTime, TuiHub.Protos.Librarian.Sephirah.V1.AppRunTime>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new InternalID { Id = src.Id }))
            .ForMember(dest => dest.AppId, opt => opt.MapFrom(src => new InternalID { Id = src.AppId }))
            .ForMember(dest => dest.DeviceId, opt => opt.MapFrom(src => new InternalID { Id = src.DeviceId }))
            .ForMember(dest => dest.RunTime, opt => opt.MapFrom(src => new TimeRange
            {
                StartTime = Timestamp.FromDateTime(src.StartDateTime.ToUniversalTime()),
                Duration = Duration.FromTimeSpan(src.Duration)
            }));

        // FileMetadata: DB -> Protobuf
        CreateMap<FileMetadata, TuiHub.Protos.Librarian.V1.FileMetadata>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new InternalID { Id = src.Id }))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty))
            .ForMember(dest => dest.Sha256, opt => opt.MapFrom(src => ByteString.CopyFrom(src.Sha256)))
            .ForMember(dest => dest.CreateTime,
                opt => opt.MapFrom(src => Timestamp.FromDateTime(src.CreatedAt.ToUniversalTime())));
    }
}