using GoldenCrownApi.Common;
using GoldenCrownApi.Database;
using GoldenCrownApi.Dtos.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrownApi.Features.Finance.Queries.GetTransactionHistory
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
            var accountsQuery = _db.Accounts.Where(x => x.UserId == request.UserId);
            if (request.CurrencyId is not null)
            {
                accountsQuery = accountsQuery.Where(x => x.CurrencyId == request.CurrencyId);
            }
            var accountIds = await accountsQuery.Select(x => x.Id).ToListAsync(cancellationToken);
            if (accountIds.Count == 0)
            {
                return Result<List<TransactionInfo>>.Failure("Счёт пользователя не найден");
            }
            if (request.To < request.From)
            {
                return Result<List<TransactionInfo>>.Failure("Некорректный диапазон дат.");
            }
            var transactionQuery = _db.Transactions
                .Where(x => (accountIds.Contains(x.SenderAccountId) || accountIds.Contains(x.ReceiverAccountId)));
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
                    IsSender = accountIds.Contains(x.SenderAccountId),
                    SenderName = x.SenderAccount.User.Name,
                    ReceiverName = x.ReceiverAccount.User.Name,
                    AccountCurrency = x.SenderAccount.Currency.Name,
                    Date = x.Date,
                    Amount = x.Amount
                }).ToListAsync(cancellationToken);
            return Result<List<TransactionInfo>>.Success(transactionInfos);
        }
    }
}
