using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Librarian.Common.Constants;

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
            .ForMember(dest => dest.AltNames, opt => opt.MapFrom(src => src.NameAlternatives))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
            .ReverseMap();
    }
}