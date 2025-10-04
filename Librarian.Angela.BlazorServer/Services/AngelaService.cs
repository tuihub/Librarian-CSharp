using System.Text;
using System.Text.Json;
using Librarian.Angela.BlazorServer.Services.Models;
using Microsoft.Extensions.Options;

namespace Librarian.Angela.BlazorServer.Services;

public interface IAngelaService
{
    Task<LoginResponse?> LoginAsync(string username, string password);
    Task<LoginResponse?> RefreshTokenAsync(string refreshToken);
    Task<List<SentinelSummary>?> GetSentinelsAsync();
    Task<List<StoreAppSummary>?> SearchStoreAppsAsync(string? nameFilter = null);
    Task<CheckLocalAdminResponse?> CheckLocalAdminAsync();
}

public class AngelaService : IAngelaService
{
    private readonly AngelaApiConfig _config;
    private readonly HttpClient _httpClient;
    private readonly ILogger<AngelaService> _logger;

    public AngelaService(HttpClient httpClient, IOptions<AngelaApiConfig> config,
        ILogger<AngelaService> logger)
    {
        _httpClient = httpClient;
        _config = config.Value;
        _logger = logger;
    }

    public async Task<LoginResponse?> LoginAsync(string username, string password)
    {
        try
        {
            var request = new LoginRequest
            {
                Username = username,
                Password = password
            };

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Use the gRPC JSON transcoding endpoint for Angela
            var response = await _httpClient.PostAsync($"{_config.BaseUrl}/api/v1/auth/token", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                return loginResponse;
            }

            _logger.LogWarning("Angela login failed with status: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during Angela login");
            return null;
        }
    }

    public async Task<LoginResponse?> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var request = new { };

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Add authorization header with refresh token
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", refreshToken);

            var response = await _httpClient.PostAsync($"{_config.BaseUrl}/api/v1/auth/refresh", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                return loginResponse;
            }

            _logger.LogWarning("Angela token refresh failed with status: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during Angela token refresh");
            return null;
        }
    }

    public async Task<List<SentinelSummary>?> GetSentinelsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_config.BaseUrl}/api/v1/sentinels");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var sentinelsResponse = JsonSerializer.Deserialize<dynamic>(responseContent);
                
                // Parse the response - this would need to match the actual proto JSON structure
                var sentinels = new List<SentinelSummary>();
                
                _logger.LogInformation("Retrieved sentinels successfully");
                return sentinels;
            }

            _logger.LogWarning("Get sentinels failed with status: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching sentinels");
            return null;
        }
    }

    public async Task<List<StoreAppSummary>?> SearchStoreAppsAsync(string? nameFilter = null)
    {
        try
        {
            var url = $"{_config.BaseUrl}/api/v1/store-apps";
            if (!string.IsNullOrEmpty(nameFilter))
            {
                url += $"?nameLike={Uri.EscapeDataString(nameFilter)}";
            }

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var storeAppsResponse = JsonSerializer.Deserialize<dynamic>(responseContent);
                
                // Parse the response - this would need to match the actual proto JSON structure
                var storeApps = new List<StoreAppSummary>();
                
                _logger.LogInformation("Retrieved store apps successfully");
                return storeApps;
            }

            _logger.LogWarning("Search store apps failed with status: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching store apps");
            return null;
        }
    }

    public async Task<CheckLocalAdminResponse?> CheckLocalAdminAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_config.BaseUrl}/api/v1/auth/check-local-admin");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var checkResponse = JsonSerializer.Deserialize<CheckLocalAdminResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                _logger.LogInformation("CheckLocalAdmin returned: IsLocalAdmin={IsLocalAdmin}, Username={Username}", 
                    checkResponse?.IsLocalAdmin, checkResponse?.Username);
                return checkResponse;
            }

            _logger.LogWarning("CheckLocalAdmin failed with status: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking LocalAdmin status");
            return null;
        }
    }
}