using GoldenCrown.Common;
using GoldenCrown.Database;
using GoldenCrown.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Features.Users.Commands.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterCommand, Result>
    {
        private readonly GoldenCrownDbContext _db;

        public RegisterUserHandler(GoldenCrownDbContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (await _db.Users.AnyAsync(u => request.Login == u.Login, cancellationToken))
            {
                return Result.Failure("Пользователь с таким логином уже существует.");
            }
            var user = new User
            {
                Login = request.Login,
                Password = request.Password,
                Name = request.Name
            };
            _db.Users.Add(user);
            _db.Accounts.Add(new Account { User = user });
            await _db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
