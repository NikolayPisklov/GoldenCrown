using GoldenCrown.Database;
using GoldenCrown.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    public interface IUserService
    {
        public Task<bool> RegisterAsync(string login, string password, string name);
    }
    public class UserService : IUserService
    {
        private readonly GoldenCrownDbContext _db;
        private readonly IAccountService _accountService;

        public UserService(GoldenCrownDbContext db, IAccountService accountService)
        {
            _db = db;            
            _accountService = accountService;
        }

        public async Task<bool> RegisterAsync(string login, string password, string name)
        {
            if(await _db.Users.AnyAsync(u => login == u.Login))
            {
                return false;
            }
            if(string.IsNullOrWhiteSpace(password) || password.Length < 6) 
            {
                return false;
            }
            var user = new User();
            user.Login = login;
            user.Password = password;
            user.Name = name;
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            
            var registeredUser = await _db.Users.FirstOrDefaultAsync(u => u.Login == login);
            if(registeredUser is null)
            {
                throw new Exception("Something went wrong!");
            }

            await _accountService.CreateAccountAsync(registeredUser.Id);
            return true;
        }
    }
}
