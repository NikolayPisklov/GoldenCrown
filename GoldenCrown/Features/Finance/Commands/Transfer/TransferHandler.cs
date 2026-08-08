using System.Data;
using GoldenCrown.Common;
using GoldenCrown.Database;
using GoldenCrown.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Features.Finance.Commands.Transfer
{
    public class TransferHandler : IRequestHandler<TransferCommand, Result<decimal>>
    {
        private readonly GoldenCrownDbContext _db;

        public TransferHandler(GoldenCrownDbContext db)
        {
            _db = db;
        }

        public async Task<Result<decimal>> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            await using var dbTransaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var senderAccount = await _db.Accounts.FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);
            if (senderAccount is null)
            {
                return Result<decimal>.Failure("Счёт пользователя не найден");
            }
            var receiverAccount = await _db.Accounts.FirstOrDefaultAsync(x => x.User.Login == request.ReceiverLogin, cancellationToken);
            if (receiverAccount is null)
            {
                return Result<decimal>.Failure("Счёт получателя не найден.");
            }
            if (senderAccount.Id == receiverAccount.Id)
            {
                return Result<decimal>.Failure("Нельзя перевести средства самому себе.");
            }
            if (senderAccount.Balance - request.Amount < 0)
            {
                return Result<decimal>.Failure("Недостаточно средств.");
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
            return Result<decimal>.Success(senderAccount.Balance);
        }
    }
}
