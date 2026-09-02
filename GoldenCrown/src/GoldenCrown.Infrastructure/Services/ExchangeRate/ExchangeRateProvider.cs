using GoldenCrown.Application.Abstractions;
using GoldenCrown.Domain.Common;
using System.Net.Http.Json;
using System.Text.Json;

namespace GoldenCrown.Infrastructure.Services.ExchangeRate
{
    public class ExchangeRateProvider : IExchangeRateProvider
    {
        private readonly HttpClient _httpClient;

        public ExchangeRateProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Result<decimal>> GetRateAsync(string from, string to, CancellationToken cancellationToken)
        {
            if(from == to)
            {
                return Result<decimal>.Success(1);
            }

            try
            {
                var info = await _httpClient.GetFromJsonAsync<CurrencyRate>(
                    $"v2/rate/{from}/{to}",
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
                return Result<decimal>.Failure($"Не удалось получить курс {from} → {to}. Повторите попытку позже.");
            }
            catch (JsonException)
            {
                return Result<decimal>.Failure($"Сервис курсов вернул неожиданный ответ для пары {from} → {to}.");
            }
        }
    }
}
