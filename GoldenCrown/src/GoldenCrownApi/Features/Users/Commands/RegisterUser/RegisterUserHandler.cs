using GoldenCrown.Domain.Common;
using GoldenCrown.Domain.Entities;
using GoldenCrownApi.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GoldenCrownApi.Features.Users.Commands.RegisterUser
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
            var user = User.Register(request.Login, request.Name, request.Password);
            await using var dbTransaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            _db.Users.Add(user);
            await _db.SaveChangesAsync(cancellationToken);
            _db.Accounts.Add(Account.Open(user.Id, (int)Currencies.RUB));
            await _db.SaveChangesAsync(cancellationToken);
            await dbTransaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
    }
}
