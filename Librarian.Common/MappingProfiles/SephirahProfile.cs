using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Google.Protobuf.WellKnownTypes;
using Librarian.Common.Constants;
using Librarian.Common.Converters;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;
using AppCategory = Librarian.Common.Models.Db.AppCategory;
using Enum = System.Enum;

namespace Librarian.Common.MappingProfiles;

public class SephirahProfile : Profile
{
    public SephirahProfile()
    {
        CreateMap<string, WellKnowns.AppInfoSource>()
            .ConvertUsing(s => Enum.Parse<WellKnowns.AppInfoSource>(s));

        CreateMap<AppType, Enums.AppType>()
            .ConvertUsingEnumMapping(opt => opt.MapByName());

        CreateMap<AppInfo, Models.Db.AppInfo>()
            .ForMember(dest => dest.IconImageId, opt => opt.MapFrom(src => src.IconImageId.Id))
            .ForMember(dest => dest.BackgroundImageId, opt => opt.MapFrom(src => src.BackgroundImageId.Id))
            .ForMember(dest => dest.CoverImageId, opt => opt.MapFrom(src => src.CoverImageId.Id))
            .ForMember(dest => dest.AltNames, opt => opt.MapFrom(src => src.NameAlternatives.ToList()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.ToList()))
            .ReverseMap();

        CreateMap<App, Models.Db.App>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Id))
            .ForMember(dest => dest.RevisedVersion, opt => opt.MapFrom(src => src.VersionNumber))
            .ForMember(dest => dest.RevisedAt, opt => opt.MapFrom(src => src.VersionDate.ToDateTime()))
            .ForMember(dest => dest.CreatorDeviceId, opt => opt.MapFrom(src => src.CreatorDeviceId.Id))
            .ForMember(dest => dest.AppSources, opt => opt.MapFrom(src => src.AppSources.ToDictionary(kv =>
                kv.Key.ToEnum<WellKnowns.AppInfoSource>(), kv => kv.Value)))
            .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.Public))
            .ForMember(dest => dest.BoundStoreAppId, opt => opt.MapFrom(src => src.BoundStoreAppId.Id == 0 ? (long?)null : src.BoundStoreAppId.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToEnumByString<Enums.AppType>()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.IconImageUrl, opt => opt.MapFrom(src => src.IconImageUrl))
            .ForMember(dest => dest.IconImageId, opt => opt.MapFrom(src => src.IconImageId.Id))
            .ForMember(dest => dest.BackgroundImageUrl, opt => opt.MapFrom(src => src.BackgroundImageUrl))
            .ForMember(dest => dest.BackgroundImageId, opt => opt.MapFrom(src => src.BackgroundImageId.Id))
            .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src => src.CoverImageUrl))
            .ForMember(dest => dest.CoverImageId, opt => opt.MapFrom(src => src.CoverImageId.Id))
            .ForMember(dest => dest.Developer, opt => opt.MapFrom(src => src.Developer))
            .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.Publisher))
            .ForMember(dest => dest.AltNames, opt => opt.MapFrom(src => src.NameAlternatives.ToList()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.ToList()))
            .ForMember(dest => dest.StopStoreManage, opt => opt.MapFrom(src => src.StopStoreManage))
            .ReverseMap();

        CreateMap<AppCategory, TuiHub.Protos.Librarian.Sephirah.V1.AppCategory>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new InternalID { Id = src.Id }))
            .ForMember(dest => dest.VersionNumber, opt => opt.MapFrom(src => 0UL))
            .ForMember(dest => dest.VersionDate, opt => opt.MapFrom(src =>
                src.UpdatedAt.HasValue
                    ? Timestamp.FromDateTime(src.UpdatedAt.Value.ToUniversalTime())
                    : Timestamp.FromDateTime(src.CreatedAt.ToUniversalTime())))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.AppIds,
                opt => opt.MapFrom(src => src.Apps.Select(a => new InternalID { Id = a.Id })));
    }
}