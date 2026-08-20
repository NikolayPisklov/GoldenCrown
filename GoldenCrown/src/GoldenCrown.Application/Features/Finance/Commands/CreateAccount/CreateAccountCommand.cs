using GoldenCrown.Domain.Common;
using MediatR;

namespace GoldenCrown.Application.Features.Finance.Commands.CreateAccount
{
    public record CreateAccountCommand
    (
        int UserId,
        int CurrencyId
    ) : IRequest<Result>;
}
