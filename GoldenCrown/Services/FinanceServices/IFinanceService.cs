using GoldenCrown.Common;
using GoldenCrown.Dtos.Account;

namespace GoldenCrown.Services.FinanceServices
{
    public interface IFinanceService
    {
        Task<Result<decimal>> DepositAsync(string token, decimal amount);
        Task<Result<decimal>> GetBalanceAsync(string token);
        Task<Result<List<TransactionInfo>>> GetTransactionHistoryAsync(string token, DateTime? from, DateTime? to, int limit, int offcet);
        Task<Result<decimal>> TransferAsync(string token, string recieverLogin, decimal amount);
    }
}
