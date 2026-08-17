using System.Data;
using GoldenCrownApi.Common;
using GoldenCrownApi.Database;
using GoldenCrownApi.Dtos.Account;
using GoldenCrownApi.Models;
using GoldenCrownApi.RabbitMQ;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrownApi.Features.Finance.Commands.Deposit
{
    public class DepositHandler : IRequestHandler<DepositCommand, Result<BalanceResponse>>
    {
        private readonly GoldenCrownDbContext _db;
        private readonly IMessagePublisher _messagePublisher;

        public DepositHandler(GoldenCrownDbContext db, IMessagePublisher messagePublisher)
        {
            _db = db;
            this._messagePublisher = messagePublisher;
        }

        public async Task<Result<BalanceResponse>> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            await using var dbTransaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var account = await _db.Accounts
                .Include(x => x.Currency)
                .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.CurrencyId == request.CurrencyId, cancellationToken);
            if (account is null)
            {
                return Result<BalanceResponse>.Failure("Счёт пользователя не найден.");
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
            await dbTransaction.CommitAsync(cancellationToken);

            await _messagePublisher.PublishAsync(new TransactionDepositEvent(
                request.UserId, 
                request.Amount, 
                account.Currency.Name), cancellationToken);
            return Result<BalanceResponse>.Success(new BalanceResponse()
            {
                Balance = account.Balance,
                AccountCurrency = account.Currency.Name
            });
        }
    }
}
