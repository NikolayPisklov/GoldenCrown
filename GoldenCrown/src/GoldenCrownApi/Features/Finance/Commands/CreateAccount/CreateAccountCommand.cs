using GoldenCrownApi.Common;
using MediatR;

namespace GoldenCrownApi.Features.Finance.Commands.CreateAccount
{
    public record CreateAccountCommand
    (
        int UserId,
        int CurrencyId
    ) : IRequest<Result>;
}
