using GoldenCrown.Application.Dtos;
using GoldenCrown.Domain.Common;
using MediatR;

namespace GoldenCrown.Application.Features.Finance.Queries.GetBalance
{
    public record GetBalanceQuery
    (
        int UserId
    ) : IRequest<Result<List<BalanceResponse>>>;
}
