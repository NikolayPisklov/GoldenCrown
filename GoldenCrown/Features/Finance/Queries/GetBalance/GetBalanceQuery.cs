using GoldenCrown.Common;
using MediatR;

namespace GoldenCrown.Features.Finance.Queries.GetBalance
{
    public record GetBalanceQuery
    (
        int UserId
    ) : IRequest<Result<decimal>>;
}
