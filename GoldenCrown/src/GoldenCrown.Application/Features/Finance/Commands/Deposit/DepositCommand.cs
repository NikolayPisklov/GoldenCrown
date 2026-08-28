using GoldenCrown.Application.Dtos;
using GoldenCrown.Domain.Common;
using MediatR;

namespace GoldenCrown.Application.Features.Finance.Commands.Deposit
{
    public record DepositCommand
    (
        int UserId,
        decimal Amount,
        int CurrencyFromId,
        int CurrencyToId
    ) : IRequest<Result<BalanceResponse>>;
}
