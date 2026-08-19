using GoldenCrown.Domain.Common;
using GoldenCrownApi.Database;
using GoldenCrownApi.Dtos.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrownApi.Features.Finance.Queries.GetBalance
{
    public class GetBalanceHandler : IRequestHandler<GetBalanceQuery, Result<List<BalanceResponse>>>
    {
        private readonly GoldenCrownDbContext _db;

        public GetBalanceHandler(GoldenCrownDbContext db)
        {
            _db = db;
        }

        public async Task<Result<List<BalanceResponse>>> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
        {
            var accounts = await (
                from a in _db.Accounts
                join c in _db.Currencies on a.CurrencyId equals c.Id
                where a.UserId == request.UserId
                select new BalanceResponse()
                {
                    Balance = a.Balance,
                    AccountCurrency = c.Name
                }).ToListAsync(cancellationToken);
            return Result<List<BalanceResponse>>.Success(accounts);
        }
    }
}
