using Librarian.ThirdParty.Contracts;
using Microsoft.Extensions.Logging;
using SteamWebAPI2.Interfaces;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.ThirdParty.Steam
{
    public partial class SteamApiService : IAccountService
    {
        public async Task<Account> GetAccountAsync(string platformAccountId, CancellationToken ct = default)
        {
            _logger.LogInformation("Getting account info for Steam ID: {SteamId}", platformAccountId);

            try
            {
                ulong steamId;
                if (!ulong.TryParse(platformAccountId, out steamId))
                {
                    _logger.LogError("Invalid Steam ID format: {SteamId}", platformAccountId);
                    throw new FormatException($"Invalid Steam ID format: {platformAccountId}");
                }

                var steamUserInterface = _webInterfaceFactory.CreateSteamWebInterface<SteamUser>(_httpClient);
                var playerSummaryResponse = await steamUserInterface.GetPlayerSummaryAsync(steamId);

                if (playerSummaryResponse?.Data == null)
                {
                    _logger.LogError("Failed to get player summary for Steam ID: {SteamId}", platformAccountId);
                    throw new Exception($"Failed to get player summary for Steam ID: {platformAccountId}");
                }

                var playerSummary = playerSummaryResponse.Data;

                _logger.LogInformation("Successfully retrieved account info for Steam ID: {SteamId}", platformAccountId);

                return new Account
                {
                    Platform = "steam",
                    PlatformAccountId = platformAccountId,
                    Name = playerSummary.Nickname,
                    ProfileUrl = playerSummary.ProfileUrl,
                    AvatarUrl = playerSummary.AvatarFullUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account info for Steam ID: {SteamId}", platformAccountId);
                throw;
            }
        }
    }
}