using AutoMapper;
using Librarian.Sephirah.Angela;
using Sentinel = Librarian.Common.Models.Db.Sentinel;

namespace Librarian.Angela.Mapping;

public class SentinelMappingProfile : Profile
{
    public SentinelMappingProfile()
    {
        // Mapping from Angela PagingRequest to generic PagingRequest (used internally)
        CreateMap<PagingRequest, PagingResponse>();

        // Mapping from Entity Sentinel to Angela Sentinel protobuf
        CreateMap<Sentinel, Sephirah.Angela.Sentinel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new InternalID { Id = src.Id }))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => new InternalID { Id = src.UserId }))
            .ForMember(dest => dest.AltUrls, opt => opt.MapFrom(src => src.AltUrls))
            .ForMember(dest => dest.RefreshToken, opt => opt.MapFrom(src => src.RefreshToken));
    }
}