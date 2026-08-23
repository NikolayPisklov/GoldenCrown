using GoldenCrown.Application.Abstractions;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace GoldenCrown.Infrastructure.Services.ExchangeRate
{
    public class CurrescyExchangeRateProvider : IExchangeRateProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CurrescyExchangeRateProvider(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<decimal> GetRateAsync(string from, string to, CancellationToken cancellationToken)
        {
            if(from == to)
            {
                return 1;
            }
            var httpClient = _httpClientFactory.CreateClient();

            var info = await httpClient.GetFromJsonAsync<CurrencyRate>(
                $"https://api.frankfurter.dev/v2/rate/{from}/{to}",
                new JsonSerializerOptions(JsonSerializerDefaults.Web),
                cancellationToken);
            if (info != null)
            {
                return info.Rate;
            }
            else
            {
                throw new NullReferenceException("External API returned null.");
            }
        }
    }
}
