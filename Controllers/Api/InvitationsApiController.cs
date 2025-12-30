using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvitationsApiController : ControllerBase
    {
        // GET: /api/InvitationsApi?eventId=e1
        [HttpGet]
        public IActionResult GetAll([FromQuery] string? eventId = null)
        {
            return Ok(new[]
            {
                new {
                    id = "i1",
                    eventId = eventId ?? "e1",
                    email = "guest@example.com",
                    role = "Guest",
                    status = "Pending"
                },
                new {
                    id = "i2",
                    eventId = eventId ?? "e1",
                    email = "org@example.com",
                    role = "Organizer",
                    status = "Accepted"
                }
            });
        }

        // POST: /api/InvitationsApi/send
        [HttpPost("send")]
        public IActionResult SendInvitation([FromBody] object payload)
        {
            return Ok(new
            {
                message = "Invitation sent",
                status = "Pending",
                payload
            });
        }

        // POST: /api/InvitationsApi/{id}/accept
        [HttpPost("{id}/accept")]
        public IActionResult AcceptInvitation(string id)
        {
            return Ok(new
            {
                message = "Invitation accepted",
                invitationId = id,
                participantCreated = new
                {
                    participantId = "p_new",
                    role = "Guest"
                }
            });
        }

        // POST: /api/InvitationsApi/{id}/decline
        [HttpPost("{id}/decline")]
        public IActionResult DeclineInvitation(string id)
        {
            return Ok(new
            {
                message = "Invitation declined",
                invitationId = id
            });
        }
    }
}
