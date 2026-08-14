using GoldenCrownApi.Common;
using GoldenCrownApi.Dtos.Account;
using MediatR;

namespace GoldenCrownApi.Features.Finance.Commands.Deposit
{
    public record DepositCommand
    (
        int UserId,
        decimal Amount,
        int CurrencyId
    ) : IRequest<Result<BalanceResponse>>;
}
