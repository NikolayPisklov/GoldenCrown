using GoldenCrown.Application.Abstractions;
using GoldenCrown.Domain.Common;
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

        public async Task<Result<decimal>> GetRateAsync(string from, string to, CancellationToken cancellationToken)
        {
            if(from == to)
            {
                return Result<decimal>.Success(1);
            }
            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                var info = await httpClient.GetFromJsonAsync<CurrencyRate>(
                    $"https://api.frankfurter.dev/v2/rate/{from}/{to}",
                    new JsonSerializerOptions(JsonSerializerDefaults.Web),
                    cancellationToken);
                if (info is null)
                {
                    return Result<decimal>.Failure($"Не удалось получить курс {from} → {to}.");
                }
                return Result<decimal>.Success(info.Rate);
            }
            catch (HttpRequestException)
            {
                return Result<decimal>.Failure($"Курс {from} → {to} недоступен. Возможно, эта пара валют не поддерживается сервисом курсов.");
            }
            catch (JsonException)
            {
                return Result<decimal>.Failure($"Сервис курсов вернул неожиданный ответ для пары {from} → {to}.");
            }
        }
    }
}
