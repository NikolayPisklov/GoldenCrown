using GoldenCrown.Application.Abstractions;
using GoldenCrown.Application.Dtos;
using GoldenCrown.Contracts.Events;
using GoldenCrown.Domain.Common;
using GoldenCrown.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GoldenCrown.Application.Features.Finance.Commands.Deposit
{
    public class DepositHandler : IRequestHandler<DepositCommand, Result<BalanceResponse>>
    {
        private readonly IApplicationDbContext _db;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IExchangeRateProvider _exchangeRateProvider;

        public DepositHandler(IApplicationDbContext db, IMessagePublisher messagePublisher, IExchangeRateProvider exchangeRateProvider)
        {
            _db = db;
            _messagePublisher = messagePublisher;
            _exchangeRateProvider = exchangeRateProvider;
        }

        public async Task<Result<BalanceResponse>> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            var currencyFrom = (Currencies)request.CurrencyFromId;
            var currencyTo = (Currencies)request.CurrencyToId;
            if (!Enum.IsDefined(currencyFrom))
            {
                return Result<BalanceResponse>.Failure($"Валюта с идентификатором {request.CurrencyFromId} не найдена.");
            }
            if (!Enum.IsDefined(currencyTo))
            {
                return Result<BalanceResponse>.Failure($"Валюта с идентификатором {request.CurrencyToId} не найдена.");
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

            var account = await _db.Accounts
                .FirstOrDefaultAsync(a => a.UserId == request.UserId && a.CurrencyId == request.CurrencyToId, cancellationToken);
            if (account is null)
            {
                return Result<BalanceResponse>.Failure("Счёт пользователя не найден.");
            }

            account.Deposit(convertedAmount);

            var transaction = Transaction.CreateDeposit(account, request.Amount, rate, currencyFrom.ToString(), currencyTo.ToString(), convertedAmount);
            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync(cancellationToken);
            await dbTransaction.CommitAsync(cancellationToken);

            await _messagePublisher.PublishAsync(new DepositEvent(
                TransactionId: transaction.Id,
                UserId: request.UserId,
                Amount: request.Amount,
                CurrencyFrom: currencyFrom.ToString(),
                ConvertedAmount: convertedAmount,
                CurrencyTo: currencyTo.ToString(),
                Rate: rate,
                Date: transaction.Date), cancellationToken);
            return Result<BalanceResponse>.Success(new BalanceResponse()
            {
                Balance = account.Balance,
                AccountCurrency = currencyTo.ToString()
            });
        }
    }
}
