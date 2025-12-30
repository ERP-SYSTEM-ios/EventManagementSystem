using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventCommentsApiController : ControllerBase
    {
        public record AddCommentRequest(string EventId, string UserId, string Comment);

        // GET: /api/EventCommentsApi?eventId=e1
        [HttpGet]
        public IActionResult GetAll([FromQuery] string? eventId = null)
        {
            return Ok(new[]
            {
                new {
                    id = "cm1",
                    eventId = eventId ?? "e1",
                    userId = "u1",
                    comment = "We need projector.",
                    createdAt = "2025-12-29T00:00:00Z"
                },
                new {
                    id = "cm2",
                    eventId = eventId ?? "e1",
                    userId = "u2",
                    comment = "Room confirmed.",
                    createdAt = "2025-12-29T01:00:00Z"
                }
            });
        }

        // POST: /api/EventCommentsApi
        [HttpPost]
        public IActionResult Add([FromBody] AddCommentRequest req)
        {
            return Ok(new
            {
                message = "Comment added",
                created = new
                {
                    id = "cm_new",
                    req.EventId,
                    req.UserId,
                    req.Comment,
                    createdAt = DateTime.UtcNow
                }
            });
        }

        // DELETE: /api/EventCommentsApi/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            return NoContent();
        }
    }
}
