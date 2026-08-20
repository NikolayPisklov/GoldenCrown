using GoldenCrown.Application.Abstractions;
using GoldenCrown.Application.Dtos;
using GoldenCrown.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Application.Features.Finance.Queries.GetTransactionHistory
{
    public class GetTransactionHistoryHandler : IRequestHandler<GetTransactionHistoryQuery, Result<List<TransactionInfo>>>
    {
        private readonly IApplicationDbContext _db;

        public GetTransactionHistoryHandler(IApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Result<List<TransactionInfo>>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            //todo validation for currency id to not be null (in validator)
            var accountId = await _db.Accounts.Where(x => x.UserId == request.UserId
                && x.CurrencyId == request.CurrencyId)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (accountId == null)
            {
                return Result<List<TransactionInfo>>.Failure("Счёт пользователя не найден");
            }
            if (request.To < request.From)
            {
                return Result<List<TransactionInfo>>.Failure("Некорректный диапазон дат.");
            }
            var transactionQuery = _db.Transactions
                .Where(x => x.SenderAccountId == accountId || x.ReceiverAccountId == accountId);
            if (request.From is null && request.To is not null)
            {
                transactionQuery = transactionQuery.Where(x => x.Date <= request.To);
            }
            else if (request.From is not null && request.To is null)
            {
                transactionQuery = transactionQuery.Where(x => x.Date >= request.From);
            }
            else if (request.From is not null && request.To is not null)
            {
                transactionQuery = transactionQuery.Where(x => x.Date >= request.From && x.Date <= request.To);
            }
            var query =
                from t in transactionQuery
                join sa in _db.Accounts on t.SenderAccountId equals sa.Id
                join ra in _db.Accounts on t.ReceiverAccountId equals ra.Id
                join su in _db.Users on sa.UserId equals su.Id
                join ru in _db.Users on ra.UserId equals ru.Id
                join c in _db.Currencies on sa.CurrencyId equals c.Id
                select new TransactionInfo
                {
                    IsSender = sa.UserId == request.UserId,
                    SenderName = su.Name,
                    ReceiverName = ru.Name,
                    AccountCurrency = c.Name,
                    Amount = t.Amount,
                    Date = t.Date
                };
            var transactionInfos = await query.OrderByDescending(x => x.Date).ToListAsync(cancellationToken);
            return Result<List<TransactionInfo>>.Success(transactionInfos);
        }
    }
}
