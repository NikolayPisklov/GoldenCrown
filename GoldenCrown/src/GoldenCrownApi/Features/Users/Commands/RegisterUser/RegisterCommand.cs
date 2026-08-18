using GoldenCrown.Domain.Common;
using MediatR;

namespace GoldenCrownApi.Features.Users.Commands.RegisterUser
{
    public record RegisterCommand(
        string Login,
        string Password,
        string Name
    ) : IRequest<Result>;
    
}
