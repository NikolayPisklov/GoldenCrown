using GoldenCrown.Common;
using GoldenCrown.Database;
using GoldenCrown.Dtos.Account;
using GoldenCrown.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services.FinanceServices
{
    public class FinanceService : IFinanceService
    {
        private readonly GoldenCrownDbContext _db;
        
        public FinanceService(GoldenCrownDbContext db)
        {
            _db = db;
        }


        public async Task<Result<List<TransactionInfo>>> GetTransactionHistoryAsync(int userId, DateTime? from, DateTime? to, int limit, int offcet)
        {
            var account = _db.Accounts.FirstOrDefault(x => x.UserId == userId);
            if (account is null)
            {
                return Result<List<TransactionInfo>>.Failure("Счёт пользователя не найден");
            }
            if(to < from)
            {
                return Result<List<TransactionInfo>>.Failure("Некорректный диапазон дат.");
            }
            var transactionQuery = _db.Transactions
                .Where(x => (x.SenderAccountId == account.Id || x.ReceiverAccountId == account.Id));
            if(from is null && to is not null)
            {
                transactionQuery = transactionQuery.Where(x => x.Date < to);
            }
            else if(from is not null && to is null)
            {
                transactionQuery = transactionQuery.Where(x => x.Date > from);
            }
            else if(from is not null && to is not null)
            {
                transactionQuery = transactionQuery.Where(x => x.Date > from && x.Date < to);
            }
            var transactionInfos = await transactionQuery
                .OrderByDescending(x => x.Date)
                .Skip(offcet)
                .Take(limit)
                .Select(x => new TransactionInfo() 
                {
                    IsSender = x.SenderAccountId == account.Id ? true : false,
                    SenderName = x.SenderAccount.User.Name,
                    ReceiverName = x.ReceiverAccount.User.Name,
                    Date = x.Date,
                    Amount = x.Amount
                }).ToListAsync();
            return Result<List<TransactionInfo>>.Success(transactionInfos);
        }
        public async Task<Result<decimal>> TransferAsync(int userId, string recieverLogin, decimal amount)
        {
            var senderAccount = _db.Accounts.FirstOrDefault(x => x.UserId == userId);
            if (senderAccount is null)
            {
                return Result<decimal>.Failure("Счёт пользователя не найден");
            }
            if (senderAccount.Balance - amount < 0)
            {
                return Result<decimal>.Failure("Недостаточно средств.");
            }
            var recieverAccount = _db.Accounts.Where(x => x.User.Login == recieverLogin).FirstOrDefault();
            if (recieverAccount is null) 
            {
                return Result<decimal>.Failure("Счёт получателя не найден.");
            }
            senderAccount.Balance -= amount;
            recieverAccount.Balance += amount;
            var transaction = new Transaction()
            {
                SenderAccountId = senderAccount.Id,
                ReceiverAccountId = recieverAccount.Id,
                Date = DateTime.UtcNow,
                Amount = amount
            };
            _db.Transactions.Add(transaction);
            try
            {
                await _db.SaveChangesAsync();
                return Result<decimal>.Success(senderAccount.Balance);
            }
            catch (Exception) 
            {
                throw;
            }
        }
        public async Task<Result<decimal>> DepositAsync(int userId, decimal amount)
        {
            var account = _db.Accounts.FirstOrDefault(x => x.UserId == userId);
            if (account is null)
            {
                return Result<decimal>.Failure("Счёт пользователя не найден");
            }
            account.Balance += amount;
            var transaction = new Transaction()
            {
                SenderAccountId = account.Id,
                ReceiverAccountId = account.Id,
                Date = DateTime.UtcNow,
                Amount = amount
            };
            _db.Transactions.Add(transaction);
            try
            {
                await _db.SaveChangesAsync();
                return Result<decimal>.Success(account.Balance);
            }
            catch (Exception) 
            {
                throw;
            }
        }
        public async Task<Result<decimal>> GetBalanceAsync(int userId)
        {
            var account = _db.Accounts.FirstOrDefault(x => x.UserId == userId);
            if (account is null)
            {
                return Result<decimal>.Failure("Счёт пользователя не найден");
            }
            return Result<decimal>.Success(account.Balance);
        }
    }
}
