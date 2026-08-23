using GoldenCrown.Application.Dtos;
using GoldenCrown.Domain.Common;
using MediatR;

namespace GoldenCrown.Application.Features.Finance.Commands.Transfer
{
    public record TransferCommand
    (
        int UserId,
        string ReceiverLogin,
        decimal Amount,
        int FromCurrencyId,
        int ToCurrencyId
    ) : IRequest<Result<BalanceResponse>>;
}
