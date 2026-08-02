using GoldenCrown.Common;
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

        public async Task<Result> RegisterAsync(string login, string password, string name)
        {
            if (await _db.Users.AnyAsync(u => login == u.Login))
            {
                return Result.Failure("User with that login already exists.");
            }
            var user = new User
            {
                Login = login,
                Password = password,
                Name = name
            };
            _db.Users.Add(user);
            try
            {
                await _db.SaveChangesAsync();
                await CreateAccountForUserAsync(login);
            }
            catch (DbUpdateException)
            {
                throw;
            }
            return Result.Success();
        }
        public async Task<Result<string>> LoginAsync(string login, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => login == u.Login);
            if(user is null)
            {
                return Result<string>.Failure("User is not found");
            }
            if(!string.Equals(user.Password, password)) 
            {
                return Result<string>.Failure("Login or password is not correct!");
            }
            string token = Guid.NewGuid().ToString();
            DateTime expiresAt = DateTime.UtcNow.AddHours(1);
            await CreateSessionForUser(token, user.Id, expiresAt);
            return Result<string>.Success(token);
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
        private async Task CreateSessionForUser(string token, int userId, DateTime expiresAt)
        {
            var session = new Session()
            {
                UserId = userId,
                Token = token,
                ExpiresAt = expiresAt
            };
            var existingSession = await _db.Sessions.FirstOrDefaultAsync(s => s.UserId == userId);
            if(existingSession is not null)
            {
                _db.Sessions.Remove(existingSession);
            }
            _db.Sessions.Add(session);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch(DbUpdateException) 
            {
                throw;
            }
        }
    }
}
