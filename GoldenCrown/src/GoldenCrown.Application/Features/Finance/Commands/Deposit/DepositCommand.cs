using GoldenCrown.Application.Dtos;
using GoldenCrown.Domain.Common;
using MediatR;

namespace GoldenCrown.Application.Features.Finance.Commands.Deposit
{
    public record DepositCommand
    (
        int UserId,
        decimal Amount,
        int CurrencyId
    ) : IRequest<Result<BalanceResponse>>;
}
