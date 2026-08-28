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
            var currencyFrom = (Currencies)request.FromCurrencyId;
            var currencyTo = (Currencies)request.ToCurrencyId;
            if (!Enum.IsDefined(currencyFrom))
            {
                return Result<BalanceResponse>.Failure($"Валюта с идентификатором {request.FromCurrencyId} не найдена.");
            }
            if (!Enum.IsDefined(currencyTo))
            {
                return Result<BalanceResponse>.Failure($"Валюта с идентификатором {request.ToCurrencyId} не найдена.");
            }

            var rateResult = await _exchangeRateProvider.GetRateAsync(currencyFrom.ToString(), currencyTo.ToString(), cancellationToken);
            if (!rateResult)
            {
                return Result<BalanceResponse>.Failure(rateResult.ErrorMessage!);
            }
            var rate = rateResult.Value;

            var convertationResult = CurrencyConverter.Convert(request.Amount, currencyFrom, currencyTo, rate);
            if (!convertationResult)
            {
                return Result<BalanceResponse>.Failure(convertationResult.ErrorMessage!);
            }
            var convertedAmount = convertationResult.Value;

            await using var dbTransaction = await _db.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var senderAccount = await GetAccountAsync(request.UserId, request.FromCurrencyId, cancellationToken);
            if (senderAccount is null)
            {
                return Result<BalanceResponse>.Failure("Счёт отправителя в выбранной валюте не найден.");
            }
            var receiverAccount = await GetAccountAsync(request.ReceiverLogin, request.ToCurrencyId, cancellationToken);
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
            receiverAccount.Deposit(convertedAmount);

            var transaction = Transaction.CreateTransfer(senderAccount, receiverAccount, request.Amount, convertedAmount, rate, currencyFrom.ToString(), currencyTo.ToString());
            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync(cancellationToken);
            await dbTransaction.CommitAsync(cancellationToken);

            await _messagePublisher.PublishAsync(new TransferEvent(
                TransactionId: transaction.Id,
                SenderId: senderAccount.UserId,
                ReceiverId: receiverAccount.UserId,
                Amount: request.Amount,
                CurrencyFrom: currencyFrom.ToString(),
                ConvertedAmount: convertedAmount,
                CurrencyTo: currencyTo.ToString(),
                Rate: rate,
                Date: transaction.Date
                ), cancellationToken);
            return Result<BalanceResponse>.Success(new BalanceResponse { Balance = senderAccount.Balance, AccountCurrency = currencyFrom.ToString() });
        }

        private async Task<Account?> GetAccountAsync(int userId, int currencyId, CancellationToken cancellationToken)
        {
            return await _db.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId && a.CurrencyId == currencyId, cancellationToken);
        }
        private async Task<Account?> GetAccountAsync(string login, int currencyId, CancellationToken cancellationToken)
        {
            return await (
                from a in _db.Accounts
                join u in _db.Users on a.UserId equals u.Id
                where u.Login == login && a.CurrencyId == currencyId
                select a
                ).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
