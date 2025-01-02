using System.Threading.RateLimiting;

namespace Librarian.ThirdParty.Handlers
{
    public class FuncRateLimitedHandler<T> : IAsyncDisposable
    {
        private readonly RateLimiter _limiter;
        private readonly bool _blockWaiting;

        public FuncRateLimitedHandler(RateLimiter limiter, bool blockWaiting = false)
        {
            _limiter = limiter;
            _blockWaiting = blockWaiting;
        }

        public async Task<T> ExecuteAsync(Func<Task<T>> func, CancellationToken cancellationToken)
        {
            // Wait until the rate limit is available
            if (_blockWaiting)
            {
                await _limiter.AcquireAsync(permitCount: 0, cancellationToken);
            }

            using RateLimitLease lease = await _limiter.AcquireAsync(permitCount: 1, cancellationToken);

            if (lease.IsAcquired)
            {
                return await func();
            }

            throw new RateLimitExceededException("Rate limit exceeded. Please try again later.");
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await _limiter.DisposeAsync().ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }
    }

    public class RateLimitExceededException : Exception
    {
        public RateLimitExceededException(string message) : base(message) { }
    }
}
