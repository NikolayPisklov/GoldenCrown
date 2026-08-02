using GoldenCrown.Common;

namespace GoldenCrown.Services.FinanceServices
{
    public interface IFinanceService
    {
        Task<Result<decimal>> DepositAsync(string token, decimal amount);
        Task<Result<decimal>> GetBalanceAsync(string token);
        Task<Result<decimal>> TransferAsync(string token, string recieverLogin, decimal amount);
    }
}
