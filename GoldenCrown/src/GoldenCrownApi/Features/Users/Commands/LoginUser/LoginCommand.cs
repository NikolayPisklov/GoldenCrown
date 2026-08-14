using GoldenCrown.Common;
using MediatR;

namespace GoldenCrown.Features.Users.Commands.LoginUser
{
    public record LoginCommand
    (
        string Login,
        string Password
    ) : IRequest<Result<string>>;
}
