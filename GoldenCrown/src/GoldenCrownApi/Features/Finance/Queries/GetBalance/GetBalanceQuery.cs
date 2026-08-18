using GoldenCrown.Domain.Common;
using GoldenCrownApi.Dtos.Account;
using MediatR;

namespace GoldenCrownApi.Features.Finance.Queries.GetBalance
{
    public record GetBalanceQuery
    (
        int UserId
    ) : IRequest<Result<List<BalanceResponse>>>;
}
