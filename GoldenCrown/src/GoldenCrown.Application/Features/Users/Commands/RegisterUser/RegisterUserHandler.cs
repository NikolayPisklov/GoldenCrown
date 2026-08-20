using GoldenCrown.Application.Abstractions;
using GoldenCrown.Domain.Common;
using GoldenCrown.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GoldenCrown.Application.Features.Users.Commands.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterCommand, Result>
    {
        private readonly IApplicationDbContext _db;

        public RegisterUserHandler(IApplicationDbContext db)
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
            await using var dbTransaction = await _db.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            _db.Users.Add(user);
            await _db.SaveChangesAsync(cancellationToken);
            _db.Accounts.Add(Account.Open(user.Id, (int)Currencies.RUB));
            await _db.SaveChangesAsync(cancellationToken);
            await dbTransaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
    }
}
