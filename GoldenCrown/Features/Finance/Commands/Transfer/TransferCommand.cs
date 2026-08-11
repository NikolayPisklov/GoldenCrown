using GoldenCrown.Common;
using GoldenCrown.Dtos.Account;
using MediatR;

namespace GoldenCrown.Features.Finance.Commands.Transfer
{
    public record TransferCommand
    (
        int UserId,
        string ReceiverLogin,
        decimal Amount,
        int CurrencyId
    ) : IRequest<Result<BalanceResponse>>;
}
