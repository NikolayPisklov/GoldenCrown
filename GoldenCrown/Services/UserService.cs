using GoldenCrown.Database;
using GoldenCrown.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
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
            if (await _db.Users.AnyAsync(u => login == u.Login))
            {
                return false;
            }
            var user = new User
            {
                Login = login,
                Password = password,
                Name = name
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            await CreateAccountForUserAsync(login);
            return true;
        }
        public async Task<string> LoginAsync(string login, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => login == u.Login);
            if(user is null)
            {
                throw new InvalidOperationException("User is not found.");
            }
            if(!string.Equals(user.Password, password)) 
            {
                throw new InvalidOperationException("Login or password is not correct!");
            }
            string token = Guid.NewGuid().ToString();
            DateTime expiresAt = DateTime.UtcNow.AddHours(1);
            var session = new Session() 
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = expiresAt
            };
            _db.Sessions.Add(session);
            try 
            {  
                await _db.SaveChangesAsync();
                return token;
            }
            catch (DbUpdateException exception) 
            {
                throw new InvalidOperationException("You are already logged in.", exception);
            }
        }
        private async Task CreateAccountForUserAsync(string login)
        {
            var registeredUser = await _db.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (registeredUser is null)
            {
                throw new InvalidOperationException("User is not found.");
            }
            await _accountService.CreateAccountAsync(registeredUser.Id);
        }
    }
}
