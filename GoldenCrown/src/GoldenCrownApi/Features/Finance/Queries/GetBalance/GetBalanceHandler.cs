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
            var accounts = await _db.Accounts
                .Where(a => a.UserId == request.UserId)
                .Select(a => new BalanceResponse()
                {
                    Balance = a.Balance,
                    AccountCurrency = a.Currency.Name
                })
                .ToListAsync(cancellationToken);
            return Result<List<BalanceResponse>>.Success(accounts);
        }
    }
}
