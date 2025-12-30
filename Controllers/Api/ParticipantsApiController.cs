using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantsApiController : ControllerBase
    {
        // GET: /api/ParticipantsApi?eventId=123
        [HttpGet]
        public IActionResult GetAll([FromQuery] string? eventId = null)
        {
            return Ok(new[]
            {
                new { id = "p1", eventId = eventId ?? "1", name = "Ali Veli", email = "ali@example.com", role = "Guest" },
                new { id = "p2", eventId = eventId ?? "1", name = "Ayşe Yılmaz", email = "ayse@example.com", role = "Organizer" }
            });
        }

        // POST: /api/ParticipantsApi
        [HttpPost]
        public IActionResult Add([FromBody] object payload)
        {
            return Ok(new { message = "Participant added (demo response)", payload });
        }

        // DELETE: /api/ParticipantsApi/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            return NoContent();
        }
    }
}
