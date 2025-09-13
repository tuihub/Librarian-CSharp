using System.ComponentModel.DataAnnotations;

namespace Librarian.Angela.BlazorServer.Services.Models;

public class LoginRequest
{
    [Required] public string Username { get; set; } = string.Empty;

    [Required] public string Password { get; set; } = string.Empty;

    public DeviceId? DeviceId { get; set; }
}

public class DeviceId
{
    public long Id { get; set; }
}

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class SephirahApiConfig
{
    public string BaseUrl { get; set; } = string.Empty;
}

public class AngelaApiConfig
{
    public string BaseUrl { get; set; } = string.Empty;
}

// Simple models for Angela service responses
public class SentinelSummary
{
    public long Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string[] AltUrls { get; set; } = Array.Empty<string>();
    public string GetTokenUrlPath { get; set; } = string.Empty;
    public string DownloadFileUrlPath { get; set; } = string.Empty;
}

public class StoreAppSummary
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Developer { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string[] Tags { get; set; } = Array.Empty<string>();
    public bool IsPublic { get; set; }
}