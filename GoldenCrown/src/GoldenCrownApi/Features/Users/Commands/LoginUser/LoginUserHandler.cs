using GoldenCrown.Domain.Common;
using GoldenCrown.Domain.Entities;
using GoldenCrownApi.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrownApi.Features.Users.Commands.LoginUser
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
            await CreateSessionForUser(token, user.Id, cancellationToken);
            return Result<string>.Success(token);
        }
        private async Task CreateSessionForUser(string token, int userId, CancellationToken cancellationToken)
        {
            var existingSession = await _db.Sessions.FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
            if (existingSession is not null)
            {
                existingSession.Refresh(token, DateTime.UtcNow);
            }
            else
            {
                _db.Sessions.Add(Session.Start(userId, token, DateTime.UtcNow));
            }
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
