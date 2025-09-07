using Librarian.Common.Constants;
using Librarian.Common.Converters;
using Librarian.Porter.Configs;
using Librarian.ThirdParty.Bangumi;
using Librarian.ThirdParty.Contracts;
using Librarian.ThirdParty.Steam;
using Librarian.ThirdParty.Vndb;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Porter.Helpers;

public class AppInfoServiceResolver
{
    private readonly BangumiApiService _bangumiApiService;
    private readonly PorterConfig _porterConfig;
    private readonly IServiceProvider _serviceProvider;
    private readonly SteamApiService _steamApiService;
    private readonly VndbTcpApiService _vndbApiService;

    public AppInfoServiceResolver(
        IServiceProvider serviceProvider,
        PorterConfig porterConfig,
        SteamApiService steamApiService,
        BangumiApiService bangumiApiService,
        VndbTcpApiService vndbApiService)
    {
        _serviceProvider = serviceProvider;
        _porterConfig = porterConfig;
        _steamApiService = steamApiService;
        _bangumiApiService = bangumiApiService;
        _vndbApiService = vndbApiService;
    }

    public IAppInfoService GetService(FeatureRequest featureRequest)
    {
        WellKnowns.AppInfoSource appInfoSource;
        try
        {
            appInfoSource = featureRequest.Id.ToEnum<WellKnowns.AppInfoSource>();
        }
        catch (ArgumentException)
        {
            throw new ArgumentException($"Invalid app info source: {featureRequest.Id}");
        }

        if (!IsSourceEnabled(appInfoSource)) throw new InvalidOperationException("The source is not enabled.");
        return GetAppInfoService(appInfoSource);
    }

    private IAppInfoService GetAppInfoService(WellKnowns.AppInfoSource source)
    {
        return source switch
        {
            WellKnowns.AppInfoSource.Steam => _steamApiService,
            WellKnowns.AppInfoSource.Bangumi => _bangumiApiService,
            WellKnowns.AppInfoSource.Vndb => _vndbApiService,
            _ => throw new ArgumentException($"Unsupported source: {source}")
        };
    }

    private bool IsSourceEnabled(WellKnowns.AppInfoSource source)
    {
        return source switch
        {
            WellKnowns.AppInfoSource.Steam => _porterConfig.IsSteamEnabled,
            WellKnowns.AppInfoSource.Bangumi => _porterConfig.IsBangumiEnabled,
            WellKnowns.AppInfoSource.Vndb => _porterConfig.IsVndbEnabled,
            _ => false
        };
    }
}