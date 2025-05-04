using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Librarian.Common.Constants;
using Librarian.Common.Converters;

namespace Librarian.Common.MappingProfiles;

public class SephirahProfile : Profile
{
    public SephirahProfile()
    {
        CreateMap<string, WellKnowns.AppInfoSource>()
            .ConvertUsing(s => Enum.Parse<WellKnowns.AppInfoSource>(s));

        CreateMap<TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.AppType, Enums.AppType>()
            .ConvertUsingEnumMapping(opt => opt.MapByName());
        
        CreateMap<TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.AppInfo, Models.Db.AppInfo>()
            .ForMember(dest => dest.IconImageId, opt => opt.MapFrom(src => src.IconImageId.Id))
            .ForMember(dest => dest.BackgroundImageId, opt => opt.MapFrom(src => src.BackgroundImageId.Id))
            .ForMember(dest => dest.CoverImageId, opt => opt.MapFrom(src => src.CoverImageId.Id))
            .ForMember(dest => dest.AltNames, opt => opt.MapFrom(src => src.NameAlternatives.ToList()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.ToList()))
            .ReverseMap();
        
        CreateMap<TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.App, Models.Db.App>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.Id))
            .ForMember(dest => dest.RevisedVersion, opt => opt.MapFrom(src => src.VersionNumber))
            .ForMember(dest => dest.RevisedAt, opt => opt.MapFrom(src => src.VersionDate.ToDateTime()))
            .ForMember(dest => dest.CreatorDeviceId, opt => opt.MapFrom(src => src.CreatorDeviceId.Id))
            .ForMember(dest => dest.AppSources, opt => opt.MapFrom(src => src.AppSources.ToDictionary(kv =>
                kv.Key.ToEnum<WellKnowns.AppInfoSource>(), d => d.Value)))
            .ForMember(dest => dest.IconImageId, opt => opt.MapFrom(src => src.IconImageId.Id))
            .ForMember(dest => dest.BackgroundImageId, opt => opt.MapFrom(src => src.BackgroundImageId.Id))
            .ForMember(dest => dest.CoverImageId, opt => opt.MapFrom(src => src.CoverImageId.Id))
            .ForMember(dest => dest.AltNames, opt => opt.MapFrom(src => src.NameAlternatives.ToList()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.ToList()))
            .ReverseMap();
    }
}