using GoldenCrown.Common;

namespace GoldenCrown.Services.FinanceServices
{
    public interface IFinanceService
    {
        Task<Result<decimal>> GetBalanceAsync(string token);
    }
}
