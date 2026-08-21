using System.Data;
using GoldenCrown.Application.Abstractions;
using GoldenCrown.Application.Dtos;
using GoldenCrown.Contracts.Events;
using GoldenCrown.Domain.Common;
using GoldenCrown.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Application.Features.Finance.Commands.Deposit
{
    public class DepositHandler : IRequestHandler<DepositCommand, Result<BalanceResponse>>
    {
        private readonly IApplicationDbContext _db;
        private readonly IMessagePublisher _messagePublisher;

        public DepositHandler(IApplicationDbContext db, IMessagePublisher messagePublisher)
        {
            _db = db;
            this._messagePublisher = messagePublisher;
        }

        public async Task<Result<BalanceResponse>> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            await using var dbTransaction = await _db.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var found = await (
                from a in _db.Accounts
                join c in _db.Currencies on a.CurrencyId equals c.Id
                where a.UserId == request.UserId && a.CurrencyId == request.CurrencyId
                select new { Account = a, CurrencyName = c.Name }
                ).FirstOrDefaultAsync(cancellationToken);
            if (found is null)
            {
                return Result<BalanceResponse>.Failure("Счёт пользователя не найден.");
            }
            var account = found.Account;
            account.Deposit(request.Amount);
            var transaction = Transaction.CreateDeposit(account, request.Amount);
            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync(cancellationToken);
            await dbTransaction.CommitAsync(cancellationToken);

            await _messagePublisher.PublishAsync(new DepositEvent(
                request.UserId, 
                request.Amount,
                found.CurrencyName), cancellationToken);
            return Result<BalanceResponse>.Success(new BalanceResponse()
            {
                Balance = account.Balance,
                AccountCurrency = found.CurrencyName
            });
        }
    }
}
