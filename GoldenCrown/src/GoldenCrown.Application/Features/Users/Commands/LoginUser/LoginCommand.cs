using GoldenCrown.Domain.Common;
using MediatR;

namespace GoldenCrown.Application.Features.Users.Commands.LoginUser
{
    public record LoginCommand
    (
        string Login,
        string Password
    ) : IRequest<Result<string>>;
}
