using GoldenCrown.Common;
using MediatR;

namespace GoldenCrown.Features.Finance.Commands.Transfer
{
    public record TransferCommand
    (
        int UserId,
        string ReceiverLogin,
        decimal Amount
    ) : IRequest<Result<decimal>>;
}
