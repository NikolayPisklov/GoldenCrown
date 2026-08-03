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

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
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
