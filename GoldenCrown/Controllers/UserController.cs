using GoldenCrown.Dtos;
using GoldenCrown.Services;
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
            if(await _userService.RegisterAsync(request.Login, request.Password, request.Name)) 
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPost("/login")]
        public IActionResult Login()
        {
            return Ok();
        }

    }
}
