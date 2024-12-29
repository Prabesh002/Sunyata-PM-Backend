using Microsoft.AspNetCore.Mvc;
using Sunyata_PM_Backend.Services.Interfaces;

namespace Sunyata_PM_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = await _authService.RegisterUserAsync(request.Username, request.Password);
            if (user == null) return BadRequest(new { message = "User name already exists" });
            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _authService.AuthenticateUserAsync(request.Username, request.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = await _authService.GenerateTokenAsync(user);
            return Ok(new { token = token, role = user.Role });
        }
    }
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}