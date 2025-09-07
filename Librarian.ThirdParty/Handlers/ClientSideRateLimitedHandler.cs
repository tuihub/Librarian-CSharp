using System.Globalization;
using System.Net;
using System.Threading.RateLimiting;

namespace Librarian.ThirdParty.Handlers;

// https://learn.microsoft.com/zh-cn/dotnet/core/extensions/http-ratelimiter
public sealed class ClientSideRateLimitedHandler(
    RateLimiter limiter,
    bool blockWaiting = false,
    TimeSpan? pooledConnectionLifetime = null)
    : DelegatingHandler(CreateSocketsHttpHandler(pooledConnectionLifetime ?? TimeSpan.FromMinutes(2))), IAsyncDisposable
{
    private readonly bool _blockWaiting = blockWaiting;

    private readonly RateLimiter _limiter = limiter;

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await _limiter.DisposeAsync().ConfigureAwait(false);

        Dispose(false);
        GC.SuppressFinalize(this);
    }

    private static SocketsHttpHandler CreateSocketsHttpHandler(TimeSpan pooledConnectionLifetime)
    {
        return new SocketsHttpHandler
        {
            PooledConnectionLifetime = pooledConnectionLifetime
        };
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Wait until the rate limit is available
        if (_blockWaiting) await _limiter.AcquireAsync(0, cancellationToken);

        using var lease = await _limiter.AcquireAsync(
            1, cancellationToken);

        if (lease.IsAcquired) return await base.SendAsync(request, cancellationToken);

        var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
        if (lease.TryGetMetadata(
                MetadataName.RetryAfter, out var retryAfter))
            response.Headers.Add(
                "Retry-After",
                ((int)retryAfter.TotalSeconds).ToString(
                    NumberFormatInfo.InvariantInfo));

        return response;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing) _limiter.Dispose();
    }
}