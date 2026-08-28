using System.Collections.Frozen;

namespace GoldenCrown.Domain.Common
{
    public static class CurrencyDigits
    {
        private static readonly IReadOnlyDictionary<Currencies, int> _map = new Dictionary<Currencies, int>()
        {
            {Currencies.RUB, 2 },
            {Currencies.USD, 2 },
            {Currencies.EUR, 2 },
            {Currencies.JPY, 0 }
        };

        public static int Of(Currencies currency)
        {
            return _map[currency];
        }
    }
}
