using Librarian.Common.Constants;
using Librarian.Common.Converters;
using Librarian.Porter.Configs;
using Librarian.ThirdParty.Contracts;
using Librarian.ThirdParty.Steam;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Porter.Helpers;

public class AccountServiceResolver
{
    private readonly PorterConfig _porterConfig;
    private readonly SteamApiService _steamApiService;

    public AccountServiceResolver(
        PorterConfig porterConfig,
        SteamApiService steamApiService)
    {
        _porterConfig = porterConfig;
        _steamApiService = steamApiService;
    }

    public IAccountService GetService(FeatureRequest featureRequest)
    {
        WellKnowns.AccountPlatform platform;
        try
        {
            platform = featureRequest.Id.ToEnum<WellKnowns.AccountPlatform>();
        }
        catch (ArgumentException)
        {
            throw new ArgumentException($"Invalid account platform: {featureRequest.Id}");
        }

        if (!IsPlatformEnabled(platform)) throw new InvalidOperationException("The platform is not enabled.");

        return GetAccountService(platform);
    }

    private IAccountService GetAccountService(WellKnowns.AccountPlatform platform)
    {
        return platform switch
        {
            WellKnowns.AccountPlatform.Steam => _steamApiService,
            _ => throw new ArgumentException($"Unsupported platform: {platform}")
        };
    }

    private bool IsPlatformEnabled(WellKnowns.AccountPlatform platform)
    {
        return platform switch
        {
            WellKnowns.AccountPlatform.Steam => _porterConfig.IsSteamEnabled,
            _ => false
        };
    }
}