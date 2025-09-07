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