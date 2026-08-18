using GoldenCrown.Domain.Common;
using GoldenCrownApi.Dtos.Account;
using MediatR;

namespace GoldenCrownApi.Features.Finance.Commands.Transfer
{
    public record TransferCommand
    (
        int UserId,
        string ReceiverLogin,
        decimal Amount,
        int CurrencyId
    ) : IRequest<Result<BalanceResponse>>;
}
