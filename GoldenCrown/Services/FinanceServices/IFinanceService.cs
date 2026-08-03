using GoldenCrown.Common;
using GoldenCrown.Dtos.Account;

namespace GoldenCrown.Services.FinanceServices
{
    public interface IFinanceService
    {
        Task<Result<decimal>> DepositAsync(int userId, decimal amount);
        Task<Result<decimal>> GetBalanceAsync(int userId);
        Task<Result<List<TransactionInfo>>> GetTransactionHistoryAsync(int userId, DateTime? from, DateTime? to, int limit, int offcet);
        Task<Result<decimal>> TransferAsync(int userId, string recieverLogin, decimal amount);
    }
}
