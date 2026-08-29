using GoldenCrown.Application.Abstractions;
using GoldenCrown.Domain.Common;
using Microsoft.Extensions.Caching.Memory;

namespace GoldenCrown.Infrastructure.Services.ExchangeRate
{
    public class CachedExchangeRateProvider : IExchangeRateProvider
    {
        private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(10);

        private readonly IExchangeRateProvider _inner;
        private readonly IMemoryCache _memoryCache;

        public CachedExchangeRateProvider(IExchangeRateProvider inner, IMemoryCache memoryCache)
        {
            _inner = inner;
            _memoryCache = memoryCache;
        }

        public async Task<Result<decimal>> GetRateAsync(string from, string to, CancellationToken cancellationToken)
        {
            var cacheKey = $"rate:{from}:{to}";
            if (_memoryCache.TryGetValue(cacheKey, out decimal cachedRate))
            {
                return Result<decimal>.Success(cachedRate);
            }

            var rateResult = await _inner.GetRateAsync(from, to, cancellationToken);
            if (!rateResult)
            {
                return rateResult;
            }

            _memoryCache.Set(cacheKey, rateResult.Value, new MemoryCacheEntryOptions
            {
                Size = 1,
                Priority = CacheItemPriority.Normal,
                AbsoluteExpirationRelativeToNow = CacheLifetime
            });
            return rateResult;
        }
    }
}
