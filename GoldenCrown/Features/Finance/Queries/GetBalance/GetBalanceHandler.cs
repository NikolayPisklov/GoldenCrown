using GoldenCrown.Common;
using GoldenCrown.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Features.Finance.Queries.GetBalance
{
    public class GetBalanceHandler : IRequestHandler<GetBalanceQuery, Result<decimal>>
    {
        private readonly GoldenCrownDbContext _db;

        public GetBalanceHandler(GoldenCrownDbContext db)
        {
            _db = db;
        }

        public async Task<Result<decimal>> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
        {
            var account = await _db.Accounts.FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);
            if (account is null)
            {
                return Result<decimal>.Failure("Счёт пользователя не найден");
            }
            return Result<decimal>.Success(account.Balance);
        }
    }
}
