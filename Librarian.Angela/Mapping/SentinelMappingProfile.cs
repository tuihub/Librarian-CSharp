using AutoMapper;
using Librarian.Common.Models.Db;

namespace Librarian.Angela.Mapping;

public class SentinelMappingProfile : Profile
{
    public SentinelMappingProfile()
    {
        // Mapping from Angela PagingRequest to generic PagingRequest (used internally)
        CreateMap<Librarian.Sephirah.Angela.PagingRequest, Librarian.Sephirah.Angela.PagingResponse>();

        // Mapping from Entity Sentinel to Angela Sentinel protobuf
        CreateMap<Sentinel, Librarian.Sephirah.Angela.Sentinel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new Librarian.Sephirah.Angela.InternalID { Id = src.Id }))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => new Librarian.Sephirah.Angela.InternalID { Id = src.UserId }))
            .ForMember(dest => dest.AltUrls, opt => opt.MapFrom(src => src.AltUrls))
            .ForMember(dest => dest.RefreshToken, opt => opt.MapFrom(src => src.RefreshToken));
    }
}