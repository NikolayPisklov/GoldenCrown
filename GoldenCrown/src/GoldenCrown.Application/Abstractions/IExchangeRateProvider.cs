using GoldenCrown.Domain.Common;

namespace GoldenCrown.Application.Abstractions
{
    public interface IExchangeRateProvider
    {
        Task<Result<decimal>> GetRateAsync(string from, string to, CancellationToken cancellationToken);
    }
}
