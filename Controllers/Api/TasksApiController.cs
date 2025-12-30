using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksApiController : ControllerBase
    {
        // GET: /api/TasksApi?eventId=123
        [HttpGet]
        public IActionResult GetAll([FromQuery] string? eventId = null)
        {
            return Ok(new[]
            {
                new { id = "t1", eventId = eventId ?? "1", title = "Prepare slides", status = "Pending" },
                new { id = "t2", eventId = eventId ?? "1", title = "Book room", status = "Completed" }
            });
        }

        // GET: /api/TasksApi/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            return Ok(new { id, eventId = "1", title = "Prepare slides", status = "Pending" });
        }

        // POST: /api/TasksApi
        [HttpPost]
        public IActionResult Create([FromBody] object payload)
        {
            return Ok(new { message = "Task created (demo response)", payload });
        }

        // PUT: /api/TasksApi/{id}
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] object payload)
        {
            return Ok(new { message = $"Task {id} updated (demo response)", payload });
        }

        // DELETE: /api/TasksApi/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            return NoContent();
        }
    }
}
