using System.Net.Http.Headers;
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
    Task<List<UserSummary>?> GetUsersAsync();
    Task<bool> CreateUserAsync(string username, string password, string type, string status);
    Task<bool> UpdateUserAsync(long userId, string? username, string? password, string? type, string? status);

    Task<bool> CreateSentinelAsync(long userId, string url, string getTokenPath, string downloadPath,
        string refreshToken, string[]? altUrls = null);

    Task<bool> UpdateSentinelAsync(long sentinelId, long? userId, string? url, string? getTokenPath,
        string? downloadPath, string? refreshToken, string[]? altUrls = null);

    Task<bool> DeleteSentinelAsync(long sentinelId);
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
                new AuthenticationHeaderValue("Bearer", refreshToken);

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
                var sentinelsResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                var sentinels = new List<SentinelSummary>();

                if (sentinelsResponse.TryGetProperty("sentinels", out var sentinelsArray))
                    foreach (var sentinelElement in sentinelsArray.EnumerateArray())
                    {
                        var sentinel = new SentinelSummary();

                        if (sentinelElement.TryGetProperty("id", out var idObj) &&
                            idObj.TryGetProperty("id", out var idValue))
                            sentinel.Id = idValue.GetInt64();

                        if (sentinelElement.TryGetProperty("userId", out var userIdObj) &&
                            userIdObj.TryGetProperty("id", out var userIdValue))
                            sentinel.UserId = userIdValue.GetInt64();

                        if (sentinelElement.TryGetProperty("url", out var url))
                            sentinel.Url = url.GetString() ?? string.Empty;

                        if (sentinelElement.TryGetProperty("getTokenUrlPath", out var getTokenPath))
                            sentinel.GetTokenUrlPath = getTokenPath.GetString() ?? string.Empty;

                        if (sentinelElement.TryGetProperty("downloadFileUrlPath", out var downloadPath))
                            sentinel.DownloadFileUrlPath = downloadPath.GetString() ?? string.Empty;

                        if (sentinelElement.TryGetProperty("refreshToken", out var refreshToken))
                            sentinel.RefreshToken = refreshToken.GetString() ?? string.Empty;

                        if (sentinelElement.TryGetProperty("altUrls", out var altUrls))
                        {
                            var altUrlsList = new List<string>();
                            foreach (var altUrl in altUrls.EnumerateArray())
                            {
                                var urlStr = altUrl.GetString();
                                if (urlStr != null) altUrlsList.Add(urlStr);
                            }

                            sentinel.AltUrls = altUrlsList.ToArray();
                        }

                        sentinels.Add(sentinel);
                    }

                _logger.LogInformation("Retrieved {Count} sentinels successfully", sentinels.Count);
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
            if (!string.IsNullOrEmpty(nameFilter)) url += $"?nameLike={Uri.EscapeDataString(nameFilter)}";

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
                var checkResponse = JsonSerializer.Deserialize<CheckLocalAdminResponse>(responseContent,
                    new JsonSerializerOptions
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

    public async Task<List<UserSummary>?> GetUsersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_config.BaseUrl}/api/v1/users");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var usersResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                var users = new List<UserSummary>();

                if (usersResponse.TryGetProperty("users", out var usersArray))
                    foreach (var userElement in usersArray.EnumerateArray())
                    {
                        var user = new UserSummary();

                        if (userElement.TryGetProperty("id", out var idObj) &&
                            idObj.TryGetProperty("id", out var idValue))
                            user.Id = idValue.GetInt64();

                        if (userElement.TryGetProperty("username", out var username))
                            user.Username = username.GetString() ?? string.Empty;

                        if (userElement.TryGetProperty("type", out var type))
                            user.Type = type.GetString() ?? string.Empty;

                        if (userElement.TryGetProperty("status", out var status))
                            user.Status = status.GetString() ?? string.Empty;

                        users.Add(user);
                    }

                _logger.LogInformation("Retrieved {Count} users successfully", users.Count);
                return users;
            }

            _logger.LogWarning("Get users failed with status: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching users");
            return null;
        }
    }

    public async Task<bool> CreateUserAsync(string username, string password, string type, string status)
    {
        try
        {
            var request = new
            {
                user = new
                {
                    username,
                    password,
                    type,
                    status
                }
            };

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_config.BaseUrl}/api/v1/users", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("User created successfully: {Username}", username);
                return true;
            }

            _logger.LogWarning("Create user failed with status: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user");
            return false;
        }
    }

    public async Task<bool> UpdateUserAsync(long userId, string? username, string? password, string? type,
        string? status)
    {
        try
        {
            var userObj = new Dictionary<string, object>
            {
                ["id"] = new { id = userId }
            };

            if (!string.IsNullOrEmpty(username))
                userObj["username"] = username;

            if (!string.IsNullOrEmpty(password))
                userObj["password"] = password;

            if (!string.IsNullOrEmpty(type))
                userObj["type"] = type;

            if (!string.IsNullOrEmpty(status))
                userObj["status"] = status;

            var request = new
            {
                user = userObj
            };

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_config.BaseUrl}/api/v1/users/{userId}", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("User updated successfully: {UserId}", userId);
                return true;
            }

            _logger.LogWarning("Update user failed with status: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating user");
            return false;
        }
    }

    public async Task<bool> CreateSentinelAsync(long userId, string url, string getTokenPath, string downloadPath,
        string refreshToken, string[]? altUrls = null)
    {
        try
        {
            var request = new
            {
                sentinel = new
                {
                    userId = new { id = userId },
                    url,
                    altUrls = altUrls ?? Array.Empty<string>(),
                    getTokenUrlPath = getTokenPath,
                    downloadFileUrlPath = downloadPath,
                    refreshToken
                }
            };

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_config.BaseUrl}/api/v1/sentinels", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Sentinel created successfully for user: {UserId}", userId);
                return true;
            }

            _logger.LogWarning("Create sentinel failed with status: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating sentinel");
            return false;
        }
    }

    public async Task<bool> UpdateSentinelAsync(long sentinelId, long? userId, string? url, string? getTokenPath,
        string? downloadPath, string? refreshToken, string[]? altUrls = null)
    {
        try
        {
            var sentinelObj = new Dictionary<string, object>
            {
                ["id"] = new { id = sentinelId }
            };

            if (userId.HasValue && userId.Value != 0)
                sentinelObj["userId"] = new { id = userId.Value };

            if (!string.IsNullOrEmpty(url))
                sentinelObj["url"] = url;

            if (altUrls != null && altUrls.Length > 0)
                sentinelObj["altUrls"] = altUrls;

            if (!string.IsNullOrEmpty(getTokenPath))
                sentinelObj["getTokenUrlPath"] = getTokenPath;

            if (!string.IsNullOrEmpty(downloadPath))
                sentinelObj["downloadFileUrlPath"] = downloadPath;

            if (!string.IsNullOrEmpty(refreshToken))
                sentinelObj["refreshToken"] = refreshToken;

            var request = new
            {
                sentinel = sentinelObj
            };

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_config.BaseUrl}/api/v1/sentinels/{sentinelId}", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Sentinel updated successfully: {SentinelId}", sentinelId);
                return true;
            }

            _logger.LogWarning("Update sentinel failed with status: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating sentinel");
            return false;
        }
    }

    public async Task<bool> DeleteSentinelAsync(long sentinelId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_config.BaseUrl}/api/v1/sentinels/{sentinelId}");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Sentinel deleted successfully: {SentinelId}", sentinelId);
                return true;
            }

            _logger.LogWarning("Delete sentinel failed with status: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting sentinel");
            return false;
        }
    }
}