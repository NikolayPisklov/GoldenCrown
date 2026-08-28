namespace GoldenCrown.Domain.Common
{
    public static class CurrencyConverter
    {
        public static Result<decimal> Convert(decimal amount, Currencies from, Currencies to, decimal rate)
        {
            var digitsFrom = CurrencyDigits.Of(from);
            if (amount != decimal.Round(amount, digitsFrom))
            {
                return Result<decimal>.Failure($"В валюте {from} допустимо не более {digitsFrom} знаков после запятой.");
            }
            var result = decimal.Round(amount * rate, CurrencyDigits.Of(to), MidpointRounding.ToZero);
            if(result <= 0)
            {
                return Result<decimal>.Failure($"Сумма {amount} в {from} слишком мала, чтобы перевести её в {to}.");
            }
            else
            {
                return Result<decimal>.Success(result);
            }
        }
    }
}
