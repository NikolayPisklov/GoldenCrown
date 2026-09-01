using GoldenCrown.Application.Abstractions;
using GoldenCrown.Domain.Common;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RedLockNet;
using System.Globalization;

namespace GoldenCrown.Infrastructure.Services.ExchangeRate
{
    public class CachedExchangeRateProvider : IExchangeRateProvider
    {
        private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan LockExpiry = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan LockWait = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan LockRetry = TimeSpan.FromMilliseconds(200);

        private readonly IExchangeRateProvider _inner;
        private readonly IDistributedCache _distributedCache;
        private readonly IDistributedLockFactory _distributedLockFactory;
        private readonly ILogger<CachedExchangeRateProvider> _logger;

        public CachedExchangeRateProvider(IExchangeRateProvider inner, IDistributedCache distCache, IDistributedLockFactory distributedLock, ILogger<CachedExchangeRateProvider> logger)
        {
            _inner = inner;
            _distributedCache = distCache;
            _distributedLockFactory = distributedLock;
            _logger = logger;
        }

        public async Task<Result<decimal>> GetRateAsync(string from, string to, CancellationToken cancellationToken)
        {
            var cacheKey = $"rate:{from}:{to}";

            var cachedRate = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
            if (decimal.TryParse(cachedRate, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal rate))
            {
                return Result<decimal>.Success(rate);
            }

            await using var redLock = await _distributedLockFactory.CreateLockAsync("lock:" + cacheKey, LockExpiry, LockWait, LockRetry, cancellationToken);
            if (!redLock.IsAcquired)
            {
                _logger.LogWarning("Distributed lock {Resource} was not acquired ({Status}), falling back to a direct call.", cacheKey, redLock.Status);
                return await _inner.GetRateAsync(from, to, cancellationToken);
            }
            cachedRate = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
            if (decimal.TryParse(cachedRate, NumberStyles.Number, CultureInfo.InvariantCulture, out rate))
            {
                return Result<decimal>.Success(rate);
            }

            var rateResult = await _inner.GetRateAsync(from, to, cancellationToken);
            if (!rateResult)
            {
                return rateResult;
            }

            await _distributedCache.SetStringAsync(
                cacheKey,
                rateResult.Value.ToString(CultureInfo.InvariantCulture),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheLifetime },
                cancellationToken);

            return rateResult;
        }
    }
}
