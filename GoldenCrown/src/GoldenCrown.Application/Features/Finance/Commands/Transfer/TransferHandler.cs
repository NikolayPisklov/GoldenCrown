using GoldenCrown.Application.Abstractions;
using GoldenCrown.Application.Dtos;
using GoldenCrown.Contracts.Events;
using GoldenCrown.Domain.Common;
using GoldenCrown.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GoldenCrown.Application.Features.Finance.Commands.Transfer
{
    public class TransferHandler : IRequestHandler<TransferCommand, Result<BalanceResponse>>
    {
        private readonly IApplicationDbContext _db;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IExchangeRateProvider _exchangeRateProvider;

        public TransferHandler(IApplicationDbContext db, IMessagePublisher messagePublisher, IExchangeRateProvider exchangeRateProvider)
        {
            _db = db;
            _messagePublisher = messagePublisher;
            _exchangeRateProvider = exchangeRateProvider;
        }

        public async Task<Result<BalanceResponse>> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            await using var dbTransaction = await _db.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var sender = await (
                from a in _db.Accounts
                join c in _db.Currencies on a.CurrencyId equals c.Id
                where a.UserId == request.UserId && a.CurrencyId == request.FromCurrencyId
                select new { Account = a, CurrencyName = c.Name }
                ).FirstOrDefaultAsync(cancellationToken);
            if (sender is null)
            {
                return Result<BalanceResponse>.Failure("Счёт отправителя в выбранной валюте не найден.");
            }
            var senderAccount = sender.Account;
            var receiver = await (
                from a in _db.Accounts
                join c in _db.Currencies on a.CurrencyId equals c.Id
                join u in _db.Users on a.UserId equals u.Id
                where u.Login == request.ReceiverLogin && a.CurrencyId == request.ToCurrencyId
                select new {Account = a, CurrencyName = c.Name}
                ).FirstOrDefaultAsync(cancellationToken);
            if (receiver is null)
            {
                return Result<BalanceResponse>.Failure("Счёт получателя в выбранной валюте не найден.");
            }
            var receiverAccount = receiver.Account;
            if (senderAccount.Id == receiverAccount.Id)
            {
                return Result<BalanceResponse>.Failure("Нельзя перевести средства самому себе.");
            }
            var rate = await _exchangeRateProvider.GetRateAsync(sender.CurrencyName, receiver.CurrencyName, cancellationToken);
            var withdrawal = senderAccount.Withdraw(request.Amount);
            if (!withdrawal)
            {
                return Result<BalanceResponse>.Failure(withdrawal.ErrorMessage!);
            }
            var receivedAmount = request.Amount * rate;
            receiverAccount.Deposit(receivedAmount);
            var transaction = Transaction.CreateTransfer(senderAccount, receiverAccount, request.Amount);
            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync(cancellationToken);
            await dbTransaction.CommitAsync(cancellationToken);
            await _messagePublisher.PublishAsync(new TransferEvent(
                SenderId: senderAccount.UserId,
                ReceiverId: receiverAccount.UserId,
                Amount: request.Amount,
                Currency: sender.CurrencyName
                ), cancellationToken);
            return Result<BalanceResponse>.Success(new BalanceResponse { Balance = senderAccount.Balance, AccountCurrency = sender.CurrencyName });
        }
    }
}
