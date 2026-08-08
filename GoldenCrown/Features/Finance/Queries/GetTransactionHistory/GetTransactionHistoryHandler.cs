using GoldenCrown.Common;
using GoldenCrown.Database;
using GoldenCrown.Dtos.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Features.Finance.Queries.GetTransactionHistory
{
    public class GetTransactionHistoryHandler : IRequestHandler<GetTransactionHistoryQuery, Result<List<TransactionInfo>>>
    {
        private readonly GoldenCrownDbContext _db;

        public GetTransactionHistoryHandler(GoldenCrownDbContext db)
        {
            _db = db;
        }

        public async Task<Result<List<TransactionInfo>>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            var account = await _db.Accounts.FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);
            if (account is null)
            {
                return Result<List<TransactionInfo>>.Failure("Счёт пользователя не найден");
            }
            if (request.To < request.From)
            {
                return Result<List<TransactionInfo>>.Failure("Некорректный диапазон дат.");
            }
            var transactionQuery = _db.Transactions
                .Where(x => (x.SenderAccountId == account.Id || x.ReceiverAccountId == account.Id));
            if (request.From is null && request.To is not null)
            {
                transactionQuery = transactionQuery.Where(x => x.Date < request.To);
            }
            else if (request.From is not null && request.To is null)
            {
                transactionQuery = transactionQuery.Where(x => x.Date > request.From);
            }
            else if (request.From is not null && request.To is not null)
            {
                transactionQuery = transactionQuery.Where(x => x.Date > request.From && x.Date < request.To);
            }
            var transactionInfos = await transactionQuery
                .OrderByDescending(x => x.Date)
                .Skip(request.Offset)
                .Take(request.Limit)
                .Select(x => new TransactionInfo()
                {
                    IsSender = x.SenderAccountId == account.Id ? true : false,
                    SenderName = x.SenderAccount.User.Name,
                    ReceiverName = x.ReceiverAccount.User.Name,
                    Date = x.Date,
                    Amount = x.Amount
                }).ToListAsync(cancellationToken);
            return Result<List<TransactionInfo>>.Success(transactionInfos);
        }
    }
}
