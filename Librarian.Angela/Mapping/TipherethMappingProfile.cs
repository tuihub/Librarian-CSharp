using AutoMapper;

namespace Librarian.Angela.Mapping;

public class TipherethMappingProfile : Profile
{
    public TipherethMappingProfile()
    {
        // Mapping from Angela GetTokenRequest to TuiHub GetTokenRequest
        CreateMap<Librarian.Sephirah.Angela.GetTokenRequest, TuiHub.Protos.Librarian.Sephirah.V1.GetTokenRequest>();

        // Mapping from TuiHub GetTokenResponse to Angela GetTokenResponse
        CreateMap<TuiHub.Protos.Librarian.Sephirah.V1.GetTokenResponse, Librarian.Sephirah.Angela.GetTokenResponse>();

        // Mapping from Angela RefreshTokenRequest to TuiHub RefreshTokenRequest
        CreateMap<Librarian.Sephirah.Angela.RefreshTokenRequest, TuiHub.Protos.Librarian.Sephirah.V1.RefreshTokenRequest>();

        // Mapping from TuiHub RefreshTokenResponse to Angela RefreshTokenResponse
        CreateMap<TuiHub.Protos.Librarian.Sephirah.V1.RefreshTokenResponse, Librarian.Sephirah.Angela.RefreshTokenResponse>();
    }
}