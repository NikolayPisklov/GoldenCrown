namespace GoldenCrown.Application.Abstractions
{
    public interface IExchangeRateProvider
    {
        Task<decimal> GetRateAsync(string from, string to, CancellationToken cancellationToken);
    }
}
