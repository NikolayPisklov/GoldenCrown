using GoldenCrown.Common;
using MediatR;

namespace GoldenCrown.Features.Users.Commands.RegisterUser
{
    public record RegisterCommand(
        string Login,
        string Password,
        string Name
    ) : IRequest<Result>;
    
}
