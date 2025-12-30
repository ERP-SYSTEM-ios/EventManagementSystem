using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthApiController : ControllerBase
    {
        public record LoginRequest(string Username, string Password);
        public record SignupRequest(string Username, string Email, string Password, string Role);

        // POST: /api/AuthApi/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            // demo: pretend we validated user
            return Ok(new
            {
                message = "Login ok (demo)",
                user = new { id = "u1", username = req.Username, role = "Admin" },
                token = "demo-jwt-token"
            });
        }

        // POST: /api/AuthApi/signup
        [HttpPost("signup")]
        public IActionResult Signup([FromBody] SignupRequest req)
        {
            return Ok(new
            {
                message = "Signup ok (demo)",
                user = new { id = "u2", username = req.Username, email = req.Email, role = req.Role }
            });
        }

        // POST: /api/AuthApi/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logout ok (demo)" });
        }
    }
}

