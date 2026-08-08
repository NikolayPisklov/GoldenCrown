using GoldenCrown.Common;
using MediatR;

namespace GoldenCrown.Features.Finance.Commands.Deposit
{
    public record DepositCommand
    (
        int UserId,
        decimal Amount
    ) : IRequest<Result<decimal>>;
}
