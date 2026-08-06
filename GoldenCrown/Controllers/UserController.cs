using FluentValidation;
using GoldenCrown.Dtos.UserDtos;
using GoldenCrown.Services.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace GoldenCrown.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<LoginRequest> _loginValidator;
        private readonly IValidator<RegisterRequest> _registerValidator;

        public UserController(IUserService userService, IValidator<RegisterRequest> registerValidator, IValidator<LoginRequest> loginValidator)
        {
            _userService = userService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register(RegisterRequest request)
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
            var result = await _userService.RegisterAsync(request.Login, request.Password, request.Name);
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
        public async Task<IActionResult> Login(LoginRequest request)
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
            var result = await _userService.LoginAsync(request.Login, request.Password);
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
