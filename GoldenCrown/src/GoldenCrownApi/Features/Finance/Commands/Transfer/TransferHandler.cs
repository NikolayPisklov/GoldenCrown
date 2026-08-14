using System.Data;
using GoldenCrown.Common;
using GoldenCrown.Database;
using GoldenCrown.Dtos.Account;
using GoldenCrown.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Features.Finance.Commands.Transfer
{
    public class TransferHandler : IRequestHandler<TransferCommand, Result<BalanceResponse>>
    {
        private readonly GoldenCrownDbContext _db;

        public TransferHandler(GoldenCrownDbContext db)
        {
            _db = db;
        }

        public async Task<Result<BalanceResponse>> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            await using var dbTransaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var senderAccount = await _db.Accounts.Include(x => x.Currency).FirstOrDefaultAsync(x => x.UserId == request.UserId && x.CurrencyId == request.CurrencyId, cancellationToken);
            if (senderAccount is null)
            {
                return Result<BalanceResponse>.Failure("Счёт отправителя в выбранной валюте не найден.");
            }
            var receiverAccount = await _db.Accounts.FirstOrDefaultAsync(x => x.User.Login == request.ReceiverLogin && x.CurrencyId == request.CurrencyId, cancellationToken);
            if (receiverAccount is null)
            {
                return Result<BalanceResponse>.Failure("Счёт получателя в выбранной валюте не найден.");
            }
            if (senderAccount.Id == receiverAccount.Id)
            {
                return Result<BalanceResponse>.Failure("Нельзя перевести средства самому себе.");
            }
            if (senderAccount.Balance - request.Amount < 0)
            {
                return Result<BalanceResponse>.Failure("Недостаточно средств.");
            }
            senderAccount.Balance -= request.Amount;
            receiverAccount.Balance += request.Amount;
            var transaction = new Transaction()
            {
                SenderAccountId = senderAccount.Id,
                ReceiverAccountId = receiverAccount.Id,
                Date = DateTime.UtcNow,
                Amount = request.Amount
            };
            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync(cancellationToken);
            await dbTransaction.CommitAsync(cancellationToken);
            return Result<BalanceResponse>.Success(new BalanceResponse { Balance = senderAccount.Balance, AccountCurrency = senderAccount.Currency.Name});
        }
    }
}
