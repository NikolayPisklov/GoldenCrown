using GoldenCrown.Domain.Common;
using MediatR;

namespace GoldenCrown.Application.Features.Users.Commands.RegisterUser
{
    public record RegisterCommand(
        string Login,
        string Password,
        string Name
    ) : IRequest<Result>;
    
}
