using GoldenCrown.Common;
using GoldenCrown.Database;
using GoldenCrown.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Features.Finance.Commands.Deposit
{
    public class DepositHandler : IRequestHandler<DepositCommand, Result<decimal>>
    {
        private readonly GoldenCrownDbContext _db;

        public DepositHandler(GoldenCrownDbContext db)
        {
            _db = db;
        }

        public async Task<Result<decimal>> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            var account = await _db.Accounts.FirstOrDefaultAsync(x => x.UserId == request.UserId && x.CurrencyId == request.CurrencyId, cancellationToken);
            if (account is null)
            {
                return Result<decimal>.Failure("Счёт пользователя не найден.");
            }
            account.Balance += request.Amount;
            var transaction = new Transaction()
            {
                SenderAccountId = account.Id,
                ReceiverAccountId = account.Id,
                Date = DateTime.UtcNow,
                Amount = request.Amount
            };
            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync(cancellationToken);
            return Result<decimal>.Success(account.Balance);
        }
    }
}
