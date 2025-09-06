using Librarian.Angela.BlazorServer.Services.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Librarian.Angela.BlazorServer.Services
{
    public interface ISephirahAuthService
    {
        Task<LoginResponse?> LoginAsync(string username, string password);
    }

    public class SephirahAuthService : ISephirahAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly SephirahApiConfig _config;
        private readonly ILogger<SephirahAuthService> _logger;

        public SephirahAuthService(HttpClient httpClient, IOptions<SephirahApiConfig> config, ILogger<SephirahAuthService> logger)
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

                // Use the gRPC JSON transcoding endpoint
                var response = await _httpClient.PostAsync($"{_config.BaseUrl}/v1/GetToken", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    return loginResponse;
                }

                _logger.LogWarning("Login failed with status: {StatusCode}", response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login");
                return null;
            }
        }
    }
}