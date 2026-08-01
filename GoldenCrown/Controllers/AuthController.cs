using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoldenCrown.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("/register")]
        public IActionResult Register()
        {
            return Ok();
        }
        [HttpPost("/login")]
        public IActionResult Login()
        {
            return Ok();
        }

    }
}
