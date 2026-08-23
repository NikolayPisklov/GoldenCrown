namespace GoldenCrown.Infrastructure.Services.ExchangeRate
{
    internal record CurrencyRate(DateTime Date, string Base, string Quote, decimal Rate);
}
