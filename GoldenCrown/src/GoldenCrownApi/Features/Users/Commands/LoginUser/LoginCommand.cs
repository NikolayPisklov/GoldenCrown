using GoldenCrown.Domain.Common;
using MediatR;

namespace GoldenCrownApi.Features.Users.Commands.LoginUser
{
    public record LoginCommand
    (
        string Login,
        string Password
    ) : IRequest<Result<string>>;
}
