using GoldenCrown.Database;
using GoldenCrown.Models;

namespace GoldenCrown.Services
{
    public interface IAccountService
    {
        public Task CreateAccountAsync(int userId);
    }
    public class AccountService : IAccountService
    {
        private readonly GoldenCrownDbContext _db;

        public AccountService(GoldenCrownDbContext db)
        {
            _db = db;
        }

        public async Task CreateAccountAsync(int userId)
        {
            var account = new Account();
            account.UserId = userId;
            account.Balance = 0;
            _db.Accounts.Add(account);
            await _db.SaveChangesAsync();
        }
    }
}
