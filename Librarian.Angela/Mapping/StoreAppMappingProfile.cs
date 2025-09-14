using AutoMapper;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Angela.Mapping;

public class StoreAppMappingProfile : Profile
{
    public StoreAppMappingProfile()
    {
        // Mapping from Sephirah SearchStoreAppsRequest to Angela SearchStoreAppsRequest
        CreateMap<Librarian.Sephirah.Angela.SearchStoreAppsRequest, TuiHub.Protos.Librarian.Sephirah.V1.SearchStoreAppsRequest>()
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Paging));

        // Mapping from Angela PagingRequest to TuiHub PagingRequest
        CreateMap<Librarian.Sephirah.Angela.PagingRequest, TuiHub.Protos.Librarian.V1.PagingRequest>();

        // Mapping from TuiHub SearchStoreAppsResponse to Angela SearchStoreAppsResponse
        CreateMap<TuiHub.Protos.Librarian.Sephirah.V1.SearchStoreAppsResponse, Librarian.Sephirah.Angela.SearchStoreAppsResponse>()
            .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Paging))
            .ForMember(dest => dest.StoreApps, opt => opt.MapFrom(src => src.AppInfos));

        // Mapping from TuiHub PagingResponse to Angela PagingResponse
        CreateMap<TuiHub.Protos.Librarian.V1.PagingResponse, Librarian.Sephirah.Angela.PagingResponse>();

        // Mapping from TuiHub StoreAppDigest to Angela StoreApp
        CreateMap<TuiHub.Protos.Librarian.Sephirah.V1.StoreAppDigest, Librarian.Sephirah.Angela.StoreApp>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new Librarian.Sephirah.Angela.InternalID { Id = src.Id.Id }))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ShortDescription))
            .ForMember(dest => dest.CoverImageId, opt => opt.MapFrom(src => new Librarian.Sephirah.Angela.InternalID { Id = src.CoverImageId.Id }))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));

        // Mapping from Angela GetStoreAppRequest to TuiHub GetStoreAppSummaryRequest
        CreateMap<Librarian.Sephirah.Angela.GetStoreAppRequest, TuiHub.Protos.Librarian.Sephirah.V1.GetStoreAppSummaryRequest>()
            .ForMember(dest => dest.StoreAppId, opt => opt.MapFrom(src => new TuiHub.Protos.Librarian.V1.InternalID { Id = src.Id.Id }))
            .ForMember(dest => dest.AppBinaryLimit, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.AppSaveFileLimit, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.AcquiredUserLimit, opt => opt.MapFrom(src => 0));

        // Mapping from TuiHub StoreApp to Angela StoreApp
        CreateMap<TuiHub.Protos.Librarian.Sephirah.V1.StoreApp, Librarian.Sephirah.Angela.StoreApp>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new Librarian.Sephirah.Angela.InternalID { Id = src.Id.Id }))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.IconImageId, opt => opt.MapFrom(src => new Librarian.Sephirah.Angela.InternalID { Id = src.IconImageId.Id }))
            .ForMember(dest => dest.BackgroundImageId, opt => opt.MapFrom(src => new Librarian.Sephirah.Angela.InternalID { Id = src.BackgroundImageId.Id }))
            .ForMember(dest => dest.CoverImageId, opt => opt.MapFrom(src => new Librarian.Sephirah.Angela.InternalID { Id = src.CoverImageId.Id }))
            .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.Public))
            .ForMember(dest => dest.AltNames, opt => opt.MapFrom(src => src.NameAlternatives))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));

        // Mapping from Angela ListStoreAppBinariesRequest to TuiHub GetStoreAppSummaryRequest  
        CreateMap<Librarian.Sephirah.Angela.ListStoreAppBinariesRequest, TuiHub.Protos.Librarian.Sephirah.V1.GetStoreAppSummaryRequest>()
            .ForMember(dest => dest.StoreAppId, opt => opt.MapFrom(src => new TuiHub.Protos.Librarian.V1.InternalID { Id = src.StoreAppId.Id }))
            .ForMember(dest => dest.AppBinaryLimit, opt => opt.MapFrom(src => 100)) // Get binaries
            .ForMember(dest => dest.AppSaveFileLimit, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.AcquiredUserLimit, opt => opt.MapFrom(src => 0));

        // Mapping from TuiHub StoreAppBinary to Angela StoreAppBinary
        CreateMap<TuiHub.Protos.Librarian.Sephirah.V1.StoreAppBinary, Librarian.Sephirah.Angela.StoreAppBinary>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => new Librarian.Sephirah.Angela.InternalID { Id = src.Id.Id }))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.SentinelId, opt => opt.MapFrom(src => new Librarian.Sephirah.Angela.InternalID { Id = 0 })) // Default value
            .ForMember(dest => dest.SentinelGeneratedId, opt => opt.MapFrom(src => "")) // Default value
            .ForMember(dest => dest.StoreAppId, opt => opt.Ignore()); // Will be set manually
    }
}