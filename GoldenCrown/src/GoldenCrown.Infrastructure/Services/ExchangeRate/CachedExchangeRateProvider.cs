using GoldenCrown.Application.Abstractions;
using GoldenCrown.Domain.Common;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace GoldenCrown.Infrastructure.Services.ExchangeRate
{
    public class CachedExchangeRateProvider : IExchangeRateProvider
    {
        private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(10);
        private readonly IExchangeRateProvider _inner;
        private readonly IDistributedCache _distributedCache;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        public CachedExchangeRateProvider(IExchangeRateProvider inner, IDistributedCache distCache)
        {
            _inner = inner;
            _distributedCache = distCache;
        }

        public async Task<Result<decimal>> GetRateAsync(string from, string to, CancellationToken cancellationToken)
        {
            Значение читается из редиса, но децимал работает только с запятыми, а не с точкой
            var cacheKey = $"rate:{from}:{to}";
            var cachedRate = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
            if (!decimal.TryParse(cachedRate, out decimal rate))
            {
                await _semaphore.WaitAsync(cancellationToken);
                try
                {
                    cachedRate = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
                    if(decimal.TryParse(cachedRate, out rate))
                    {
                        return Result<decimal>.Success(rate);
                    }
                    var rateResult = await _inner.GetRateAsync(from, to, cancellationToken);
                    if (!rateResult)
                    {
                        return rateResult;
                    }
                    var byteValue = JsonSerializer.SerializeToUtf8Bytes(rateResult.Value);
                    await _distributedCache.SetAsync(cacheKey, byteValue, cancellationToken);
                    return rateResult;
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            else
            {                
                return Result<decimal>.Success(rate);
            }            
        }
    }
}
