using GoldenCrown.Common;
using GoldenCrown.Database;
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


        public async Task<Result<decimal>> TransferAsync(string token, string recieverLogin, decimal amount)
        {
            var accountResult = await GetUserAccount(token);
            if (!accountResult)
            {
                return Result<decimal>.Failure(accountResult.ErrorMessage!);
            }
            var senderAccount = accountResult.Value!;
            if(senderAccount.Balance - amount < 0)
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
        public async Task<Result<decimal>> DepositAsync(string token, decimal amount)
        {
            var accountResult = await GetUserAccount(token);
            if (!accountResult)
            {
                return Result<decimal>.Failure(accountResult.ErrorMessage!);
            }
            var account = accountResult.Value!;
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
        public async Task<Result<decimal>> GetBalanceAsync(string token)
        {
            var sessionResult = await IsSessionValid(token);
            if (!sessionResult) 
            {
                return Result<decimal>.Failure(sessionResult.ErrorMessage!);
            }
            var session = sessionResult.Value!;
            var account = await _db.Accounts.FirstOrDefaultAsync(x => x.UserId == session.UserId);
            if (account is null)
                throw new InvalidOperationException("Счёт пользователя не найден.");
            return Result<decimal>.Success(account.Balance);
        }


        private async Task<Result<Session>> IsSessionValid(string token)
        {
            var session = await _db.Sessions.FirstOrDefaultAsync(x => x.Token == token);
            if (session is null)
                return Result<Session>.Failure("Сессия не найдена.");
            if (session.ExpiresAt < DateTime.UtcNow)
                return Result<Session>.Failure("Время сессии истекло");
            return Result<Session>.Success(session);
        }
        private async Task<Result<Account>> GetUserAccount(string token)
        {
            var sessionResult = await IsSessionValid(token);
            if (!sessionResult)
            {
                return Result<Account>.Failure(sessionResult.ErrorMessage!);
            }
            var session = sessionResult.Value!;
            var account = await _db.Accounts.FirstOrDefaultAsync(x => x.UserId == session.UserId);
            if (account is null)
                throw new InvalidOperationException("Счёт пользователя не найден.");
            return Result<Account>.Success(account);
        }
    }
}
