using GoldenCrownApi.Common;
using GoldenCrownApi.Database;
using GoldenCrownApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrownApi.Features.Finance.Commands.CreateAccount
{
    public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, Result>
    {
        private readonly GoldenCrownDbContext _db;

        public CreateAccountHandler(GoldenCrownDbContext db)
        {
            _db = db;
        }

        public async Task<Result> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var isCurrencyExists = await _db.Currencies.AnyAsync(c => c.Id == request.CurrencyId, cancellationToken);
            if (!isCurrencyExists) 
            {
                return Result.Failure("Такой валюты не существует.");
            }
            var isDuplicateAccount = await _db.Accounts.AnyAsync(a => a.CurrencyId == request.CurrencyId && a.UserId == request.UserId, cancellationToken);
            if (isDuplicateAccount)
            {
                return Result.Failure("Вы уже имеете счёт в этой валюте.");
            }
            var account = new Account()
            {
                UserId = request.UserId,
                CurrencyId = request.CurrencyId
            };
            _db.Accounts.Add(account);
            await _db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
