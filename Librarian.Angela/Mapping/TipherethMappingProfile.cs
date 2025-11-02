using AutoMapper;
using Librarian.Sephirah.Angela;
using GetTokenResponse = TuiHub.Protos.Librarian.Sephirah.V1.GetTokenResponse;
using RefreshTokenResponse = TuiHub.Protos.Librarian.Sephirah.V1.RefreshTokenResponse;

namespace Librarian.Angela.Mapping;

public class TipherethMappingProfile : Profile
{
    public TipherethMappingProfile()
    {
        // Mapping from Angela GetTokenRequest to TuiHub GetTokenRequest
        CreateMap<GetTokenRequest, TuiHub.Protos.Librarian.Sephirah.V1.GetTokenRequest>();

        // Mapping from TuiHub GetTokenResponse to Angela GetTokenResponse
        CreateMap<GetTokenResponse, Sephirah.Angela.GetTokenResponse>();

        // Mapping from Angela RefreshTokenRequest to TuiHub RefreshTokenRequest
        CreateMap<RefreshTokenRequest, TuiHub.Protos.Librarian.Sephirah.V1.RefreshTokenRequest>();

        // Mapping from TuiHub RefreshTokenResponse to Angela RefreshTokenResponse
        CreateMap<RefreshTokenResponse, Sephirah.Angela.RefreshTokenResponse>();
    }
}