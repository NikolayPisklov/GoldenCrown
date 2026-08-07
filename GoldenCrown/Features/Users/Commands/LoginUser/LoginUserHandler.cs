using GoldenCrown.Common;
using GoldenCrown.Database;
using GoldenCrown.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Features.Users.Commands.LoginUser
{
    public class LoginUserHandler : IRequestHandler<LoginCommand, Result<string>>
    {
        private readonly GoldenCrownDbContext _db;

        public LoginUserHandler(GoldenCrownDbContext db)
        {
            _db = db;
        }

        public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => request.Login == u.Login, cancellationToken);
            if (user is null)
            {
                return Result<string>.Failure("Неверный логин или пароль.");
            }
            if (!string.Equals(user.Password, request.Password))
            {
                return Result<string>.Failure("Неверный логин или пароль.");
            }
            string token = Guid.NewGuid().ToString();
            DateTime expiresAt = DateTime.UtcNow.AddHours(1);
            await CreateSessionForUser(token, user.Id, expiresAt, cancellationToken);
            return Result<string>.Success(token);
        }
        private async Task CreateSessionForUser(string token, int userId, DateTime expiresAt, CancellationToken cancellationToken)
        {
            var session = new Session()
            {
                UserId = userId,
                Token = token,
                ExpiresAt = expiresAt
            };
            var existingSession = await _db.Sessions.FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
            if (existingSession is not null)
            {
                _db.Sessions.Remove(existingSession);
            }
            _db.Sessions.Add(session);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
