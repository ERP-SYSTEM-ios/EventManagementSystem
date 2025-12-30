using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminApiController : ControllerBase
    {
        // GET: /api/AdminApi/stats
        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            return Ok(new
            {
                totalUsers = 42,
                totalAdmins = 1,
                totalOrganizers = 10,
                totalGuests = 31,
                totalEvents = 18,
                upcomingEvents = 6,
                completedEvents = 12
            });
        }

        // GET: /api/AdminApi/users
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            return Ok(new[]
            {
                new { id = "u1", username = "admin", email = "admin@example.com", role = "Admin" },
                new { id = "u2", username = "organizer1", email = "org1@example.com", role = "Organizer" },
                new { id = "u3", username = "guest1", email = "guest1@example.com", role = "Guest" }
            });
        }
    }
}
