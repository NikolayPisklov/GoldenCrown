using GoldenCrown.Common;
using MediatR;

namespace GoldenCrown.Features.Finance.Commands.CreateAccount
{
    public record CreateAccountCommand
    (
        int UserId,
        int CurrencyId
    ) : IRequest<Result>;
}
