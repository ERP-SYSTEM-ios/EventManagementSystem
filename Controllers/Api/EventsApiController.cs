using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsApiController : ControllerBase
    {
        public record EventCreateRequest(
            string Title,
            string Description,
            string Status,
            string? LocationId,
            string? CategoryId
        );

        // GET: /api/EventsApi
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(new[]
            {
                new {
                    id = "e1",
                    title = "Tech Conference",
                    description = "Annual conference",
                    status = "Upcoming",
                    locationId = "l1",
                    categoryId = "c1"
                },
                new {
                    id = "e2",
                    title = "Workshop Day",
                    description = "Hands-on workshop",
                    status = "Completed",
                    locationId = "l2",
                    categoryId = "c2"
                }
            });
        }

        // POST: /api/EventsApi
        [HttpPost]
        public IActionResult Create([FromBody] EventCreateRequest req)
        {
            // demo: pretend created
            return Ok(new
            {
                message = "Event created (demo)",
                created = new
                {
                    id = "e_new",
                    req.Title,
                    req.Description,
                    req.Status,
                    req.LocationId,
                    req.CategoryId
                }
            });
        }
    }
}
