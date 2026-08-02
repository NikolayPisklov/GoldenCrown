using GoldenCrown.Database;
using GoldenCrown.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services.UserServices
{
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
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }
    }
}
