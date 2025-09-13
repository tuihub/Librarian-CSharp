using Grpc.Core;
using Librarian.Angela.BlazorServer.Services.Models;
using Librarian.Angela.Services;
using Librarian.Sephirah.Angela;

namespace Librarian.Angela.BlazorServer.Services;

public interface IAngelaAuthService
{
    Task<LoginResponse?> LoginAsync(string username, string password);
    Task<LoginResponse?> RefreshTokenAsync(string refreshToken);
    Task<List<Sentinel>?> GetSentinelsAsync();
    Task<List<StoreApp>?> SearchStoreAppsAsync(string? nameFilter = null);
}

public class AngelaAuthService : IAngelaAuthService
{
    private readonly AngelaService _angelaService;
    private readonly ILogger<AngelaAuthService> _logger;

    public AngelaAuthService(AngelaService angelaService, ILogger<AngelaAuthService> logger)
    {
        _angelaService = angelaService;
        _logger = logger;
    }

    public async Task<LoginResponse?> LoginAsync(string username, string password)
    {
        try
        {
            var request = new GetTokenRequest
            {
                Username = username,
                Password = password
            };

            // Create a mock ServerCallContext for the direct call
            var context = CreateMockServerCallContext();

            var response = await _angelaService.GetToken(request, context);

            return new LoginResponse
            {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken
            };
        }
        catch (RpcException ex)
        {
            _logger.LogWarning("Login failed: {Message}", ex.Status.Detail);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login");
            return null;
        }
    }

    public async Task<LoginResponse?> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var request = new RefreshTokenRequest();

            // Create a mock ServerCallContext with the refresh token
            var context = CreateMockServerCallContextWithToken(refreshToken);

            var response = await _angelaService.RefreshToken(request, context);

            return new LoginResponse
            {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken
            };
        }
        catch (RpcException ex)
        {
            _logger.LogWarning("Token refresh failed: {Message}", ex.Status.Detail);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during token refresh");
            return null;
        }
    }

    public async Task<List<Sentinel>?> GetSentinelsAsync()
    {
        try
        {
            var request = new ListSentinelsRequest
            {
                Paging = new PagingRequest { PageSize = 50, PageNum = 1 }
            };

            var context = CreateMockServerCallContext();
            var response = await _angelaService.ListSentinels(request, context);

            return response.Sentinels.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching sentinels");
            return null;
        }
    }

    public async Task<List<StoreApp>?> SearchStoreAppsAsync(string? nameFilter = null)
    {
        try
        {
            var request = new SearchStoreAppsRequest
            {
                Paging = new PagingRequest { PageSize = 50, PageNum = 1 },
                NameLike = nameFilter ?? string.Empty
            };

            var context = CreateMockServerCallContext();
            var response = await _angelaService.SearchStoreApps(request, context);

            return response.StoreApps.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching store apps");
            return null;
        }
    }

    private ServerCallContext CreateMockServerCallContext()
    {
        // For direct service calls, we need to create a minimal ServerCallContext
        // This is a simplified implementation for in-process calls
        return new MockServerCallContext();
    }

    private ServerCallContext CreateMockServerCallContextWithToken(string token)
    {
        return new MockServerCallContext(token);
    }
}

// Minimal implementation of ServerCallContext for in-process calls
public class MockServerCallContext : ServerCallContext
{
    private readonly string? _authToken;
    private readonly Metadata _requestHeaders;

    public MockServerCallContext(string? authToken = null)
    {
        _authToken = authToken;
        _requestHeaders = new Metadata();
        
        if (!string.IsNullOrEmpty(authToken))
        {
            _requestHeaders.Add("authorization", $"Bearer {authToken}");
        }
    }

    protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders) => Task.CompletedTask;
    protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions options) => throw new NotImplementedException();
    public override Task<TRequest> ReadRequestAsync<TRequest>() => throw new NotImplementedException();
    public override WriteOptions? WriteOptions { get; set; }
    public override AuthContext AuthContext => throw new NotImplementedException();
    public override DateTime Deadline => DateTime.UtcNow.AddMinutes(5);
    public override string Host => "localhost";
    public override string Method => "";
    public override string Peer => "in-process";
    public override Metadata RequestHeaders => _requestHeaders;
    public override CancellationToken CancellationToken => CancellationToken.None;
    public override Metadata ResponseTrailers => new();
    public override Status Status { get; set; }
    public override UserState UserState => new();
}