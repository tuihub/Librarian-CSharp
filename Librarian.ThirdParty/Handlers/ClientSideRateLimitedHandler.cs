using System.Globalization;
using System.Net;
using System.Threading.RateLimiting;

namespace Librarian.ThirdParty.Handlers
{
    // https://learn.microsoft.com/zh-cn/dotnet/core/extensions/http-ratelimiter
    public sealed class ClientSideRateLimitedHandler(
    RateLimiter limiter, bool blockWaiting = false, TimeSpan? pooledConnectionLifetime = null)
    : DelegatingHandler(CreateSocketsHttpHandler(pooledConnectionLifetime ?? TimeSpan.FromMinutes(2))), IAsyncDisposable
    {
        private static SocketsHttpHandler CreateSocketsHttpHandler(TimeSpan pooledConnectionLifetime) => new()
        {
            PooledConnectionLifetime = pooledConnectionLifetime
        };

        private readonly RateLimiter _limiter = limiter;
        private readonly bool _blockWaiting = blockWaiting;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Wait until the rate limit is available
            if (_blockWaiting)
            {
                await _limiter.AcquireAsync(permitCount: 0, cancellationToken);
            }

            using RateLimitLease lease = await _limiter.AcquireAsync(
                permitCount: 1, cancellationToken);

            if (lease.IsAcquired)
            {
                return await base.SendAsync(request, cancellationToken);
            }

            var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
            if (lease.TryGetMetadata(
                    MetadataName.RetryAfter, out TimeSpan retryAfter))
            {
                response.Headers.Add(
                    "Retry-After",
                    ((int)retryAfter.TotalSeconds).ToString(
                        NumberFormatInfo.InvariantInfo));
            }

            return response;
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await _limiter.DisposeAsync().ConfigureAwait(false);

            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _limiter.Dispose();
            }
        }
    }
}
