using FluentValidation;
using GoldenCrown.Dtos.UserDtos;
using GoldenCrown.Features.Users.Commands.LoginUser;
using GoldenCrown.Features.Users.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GoldenCrown.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<LoginRequest> _loginValidator;
        private readonly IMediator _mediator;

        public UserController(IValidator<RegisterRequest> registerValidator, IValidator<LoginRequest> loginValidator, IMediator mediator)
        {
            _mediator = mediator;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _registerValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                var problemDetails = new HttpValidationProblemDetails(validationResult.ToDictionary())
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation failed",
                    Detail = "One or more validation errors occurred.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }
            var result = await _mediator.Send(new RegisterCommand(request.Login, request.Password, request.Name), cancellationToken);
            if (result) 
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.ErrorMessage);
            }
        }
        [HttpPost("/login")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            var validationResult = _loginValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                var problemDetails = new HttpValidationProblemDetails(validationResult.ToDictionary())
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation failed",
                    Detail = "One or more validation errors occurred.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails);
            }
            var result = await _mediator.Send(new LoginCommand(request.Login, request.Password), cancellationToken);
            if (result)
            {
                return Ok(new LoginResponse { Token = result.Value! });
            }
            else
            {
                return NotFound();
            }
        }

    }
}
