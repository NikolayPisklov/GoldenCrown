using GoldenCrown.Common;
using GoldenCrown.Dtos.Account;
using MediatR;

namespace GoldenCrown.Features.Finance.Queries.GetBalance
{
    public record GetBalanceQuery
    (
        int UserId
    ) : IRequest<Result<List<BalanceResponse>>>;
}
