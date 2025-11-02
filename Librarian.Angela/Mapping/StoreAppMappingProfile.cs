using AutoMapper;
using Librarian.Sephirah.Angela;
using TuiHub.Protos.Librarian.Sephirah.V1;
using ListStoreAppBinariesRequest = Librarian.Sephirah.Angela.ListStoreAppBinariesRequest;
using PagingResponse = TuiHub.Protos.Librarian.V1.PagingResponse;
using SearchStoreAppsRequest = Librarian.Sephirah.Angela.SearchStoreAppsRequest;
using SearchStoreAppsResponse = TuiHub.Protos.Librarian.Sephirah.V1.SearchStoreAppsResponse;
using StoreApp = Librarian.Sephirah.Angela.StoreApp;
using StoreAppBinary = TuiHub.Protos.Librarian.Sephirah.V1.StoreAppBinary;

namespace Librarian.Angela.Mapping;

public class StoreAppMappingProfile : Profile
{
    public StoreAppMappingProfile()
    {
        // Mapping from Sephirah SearchStoreAppsRequest to Angela SearchStoreAppsRequest
        CreateMap<SearchStoreAppsRequest, TuiHub.Protos.Librarian.Sephirah.V1.SearchStoreAppsRequest>()
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Paging));

        // Mapping from Angela PagingRequest to TuiHub PagingRequest
        CreateMap<PagingRequest, TuiHub.Protos.Librarian.V1.PagingRequest>();

        // Mapping from TuiHub SearchStoreAppsResponse to Angela SearchStoreAppsResponse
        CreateMap<SearchStoreAppsResponse, Sephirah.Angela.SearchStoreAppsResponse>()
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Paging))
            .ForMember(dest => dest.StoreApps, opt => opt.MapFrom(src => src.AppInfos));

        // Mapping from TuiHub PagingResponse to Angela PagingResponse
        CreateMap<PagingResponse, Sephirah.Angela.PagingResponse>();

        // Mapping from TuiHub StoreAppDigest to Angela StoreApp
        CreateMap<StoreAppDigest, StoreApp>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new InternalID { Id = src.Id.Id }))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ShortDescription))
            .ForMember(dest => dest.CoverImageId,
                opt => opt.MapFrom(src => new InternalID { Id = src.CoverImageId.Id }))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));

        // Mapping from Angela GetStoreAppRequest to TuiHub GetStoreAppSummaryRequest
        CreateMap<GetStoreAppRequest, GetStoreAppSummaryRequest>()
            .ForMember(dest => dest.StoreAppId,
                opt => opt.MapFrom(src => new TuiHub.Protos.Librarian.V1.InternalID { Id = src.Id.Id }))
            .ForMember(dest => dest.AppBinaryLimit, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.AppSaveFileLimit, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.AcquiredUserLimit, opt => opt.MapFrom(src => 0));

        // Mapping from TuiHub StoreApp to Angela StoreApp
        CreateMap<TuiHub.Protos.Librarian.Sephirah.V1.StoreApp, StoreApp>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new InternalID { Id = src.Id.Id }))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.IconImageId, opt => opt.MapFrom(src => new InternalID { Id = src.IconImageId.Id }))
            .ForMember(dest => dest.BackgroundImageId,
                opt => opt.MapFrom(src => new InternalID { Id = src.BackgroundImageId.Id }))
            .ForMember(dest => dest.CoverImageId,
                opt => opt.MapFrom(src => new InternalID { Id = src.CoverImageId.Id }))
            .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.Public))
            .ForMember(dest => dest.AltNames, opt => opt.MapFrom(src => src.NameAlternatives))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));

        // Mapping from Angela ListStoreAppBinariesRequest to TuiHub GetStoreAppSummaryRequest  
        CreateMap<ListStoreAppBinariesRequest, GetStoreAppSummaryRequest>()
            .ForMember(dest => dest.StoreAppId,
                opt => opt.MapFrom(src => new TuiHub.Protos.Librarian.V1.InternalID { Id = src.StoreAppId.Id }))
            .ForMember(dest => dest.AppBinaryLimit, opt => opt.MapFrom(src => 100)) // Get binaries
            .ForMember(dest => dest.AppSaveFileLimit, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.AcquiredUserLimit, opt => opt.MapFrom(src => 0));

        // Mapping from TuiHub StoreAppBinary to Angela StoreAppBinary
        CreateMap<StoreAppBinary, Sephirah.Angela.StoreAppBinary>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new InternalID { Id = src.Id.Id }))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.SentinelId, opt => opt.MapFrom(src => new InternalID { Id = 0 })) // Default value
            .ForMember(dest => dest.SentinelGeneratedId, opt => opt.MapFrom(src => "")) // Default value
            .ForMember(dest => dest.StoreAppId, opt => opt.Ignore()); // Will be set manually
    }
}