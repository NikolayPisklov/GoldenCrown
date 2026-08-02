using GoldenCrown.Common;
using GoldenCrown.Database;
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

        public async Task<Result<decimal>> GetBalanceAsync(string token)
        {
            var session = await _db.Sessions.FirstOrDefaultAsync(x => x.Token == token);
            if(session is null)
                return Result<decimal>.Failure("Сессия не найдена.");
            if (session.ExpiresAt < DateTime.UtcNow) 
                return Result<decimal>.Failure("Время сессии истекло");
            var account = await _db.Accounts.FirstOrDefaultAsync(x => x.UserId == session.UserId);
            if (account is null)
                throw new InvalidOperationException("Счёт пользователя не найден.");
            return Result<decimal>.Success(account.Balance);
        }
    }
}
