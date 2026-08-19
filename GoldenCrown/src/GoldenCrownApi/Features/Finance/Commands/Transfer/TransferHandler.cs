using GoldenCrown.Domain.Common;
using GoldenCrown.Domain.Entities;
using GoldenCrownApi.Common;
using GoldenCrownApi.Database;
using GoldenCrownApi.Dtos.Account;
using GoldenCrownApi.RabbitMQ;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GoldenCrownApi.Features.Finance.Commands.Transfer
{
    public class TransferHandler : IRequestHandler<TransferCommand, Result<BalanceResponse>>
    {
        private readonly GoldenCrownDbContext _db;
        private readonly IMessagePublisher _messagePublisher;

        public TransferHandler(GoldenCrownDbContext db, IMessagePublisher messagePublisher)
        {
            _db = db;
            _messagePublisher = messagePublisher;
        }

        public async Task<Result<BalanceResponse>> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            await using var dbTransaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var sender = await (
                from a in _db.Accounts
                join c in _db.Currencies on a.CurrencyId equals c.Id
                where a.UserId == request.UserId && a.CurrencyId == request.CurrencyId
                select new { Account = a, CurrencyName = c.Name }
                ).FirstOrDefaultAsync(cancellationToken);
            if (sender is null)
            {
                return Result<BalanceResponse>.Failure("Счёт отправителя в выбранной валюте не найден.");
            }
            var senderAccount = sender.Account;
            var receiverAccount = await (
                from a in _db.Accounts
                join u in _db.Users on a.UserId equals u.Id
                where u.Login == request.ReceiverLogin && a.CurrencyId == request.CurrencyId
                select a
                ).FirstOrDefaultAsync(cancellationToken);
            if (receiverAccount is null)
            {
                return Result<BalanceResponse>.Failure("Счёт получателя в выбранной валюте не найден.");
            }
            if (senderAccount.Id == receiverAccount.Id)
            {
                return Result<BalanceResponse>.Failure("Нельзя перевести средства самому себе.");
            }
            var withdrawal = senderAccount.Withdraw(request.Amount);
            if (!withdrawal)
            {
                return Result<BalanceResponse>.Failure(withdrawal.ErrorMessage!);
            }
            receiverAccount.Deposit(request.Amount);
            var transaction = Transaction.CreateTransfer(senderAccount, receiverAccount, request.Amount);
            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync(cancellationToken);
            await dbTransaction.CommitAsync(cancellationToken);
            await _messagePublisher.PublishAsync(new TransactionEvent(
                SenderId: senderAccount.UserId,
                RecieverId: receiverAccount.UserId,
                Amount: request.Amount,
                Currency: sender.CurrencyName
                ), cancellationToken);
            return Result<BalanceResponse>.Success(new BalanceResponse { Balance = senderAccount.Balance, AccountCurrency = sender.CurrencyName });
        }
    }
}
