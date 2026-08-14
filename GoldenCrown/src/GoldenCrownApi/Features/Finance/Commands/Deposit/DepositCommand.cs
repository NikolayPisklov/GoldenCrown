using GoldenCrown.Common;
using GoldenCrown.Dtos.Account;
using MediatR;

namespace GoldenCrown.Features.Finance.Commands.Deposit
{
    public record DepositCommand
    (
        int UserId,
        decimal Amount,
        int CurrencyId
    ) : IRequest<Result<BalanceResponse>>;
}
