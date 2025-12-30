using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.Models;
using EventManagementSystem.Services;
using static Supabase.Postgrest.Constants;

namespace EventManagementSystem.Controllers
{
    public class InvitationsController : Controller
    {
        private readonly Supabase.Client _supabase;

        public InvitationsController(SupabaseService supabaseService)
        {
            _supabase = supabaseService.Client;
        }

        private bool IsLoggedIn() => HttpContext.Session.GetString("UserId") != null;

        private Guid? GetCurrentUserId()
        {
            var s = HttpContext.Session.GetString("UserId");
            return Guid.TryParse(s, out var id) ? id : (Guid?)null;
        }

        private string GetCurrentUserRole() => HttpContext.Session.GetString("Role") ?? "Guest";

        private async Task<User?> GetCurrentUserAsync()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return null;

            var resp = await _supabase
                .From<User>()
                .Where(u => u.Id == userId.Value)
                .Limit(1)
                .Get();

            return resp.Models.FirstOrDefault();
        }

        // GET: /Invitations/My
        [HttpGet]
        public async Task<IActionResult> My()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            var userId = GetCurrentUserId()!.Value;
            var me = await GetCurrentUserAsync();
            var myEmail = (me?.Email ?? "").Trim().ToLowerInvariant();

            // Query by invited_user_id
            var byUserResp = await _supabase
                .From<Invitation>()
                .Where(i => i.Status == "Pending")
                .Where(i => i.InvitedUserId == userId)
                .Get();

            var invites = byUserResp.Models.ToList();

            // Query by invited_email (only if we have email)
            if (!string.IsNullOrWhiteSpace(myEmail))
            {
                var byEmailResp = await _supabase
                    .From<Invitation>()
                    .Where(i => i.Status == "Pending")
                    .Where(i => i.InvitedEmail == myEmail)
                    .Get();

                // Merge distinct by Id
                foreach (var inv in byEmailResp.Models)
                {
                    if (!invites.Any(x => x.Id == inv.Id))
                        invites.Add(inv);
                }
            }

            // Sort newest first (if CreatedAt exists in your model)
            invites = invites
                .OrderByDescending(i => i.CreatedAt)
                .ToList();

            var eventIds = invites
                .Select(i => i.EventId)
                .Distinct()
                .ToArray();

            if (eventIds.Length > 0)
            {
                var eventsResp = await _supabase
                    .From<Event>()
                    .Filter("id", Operator.In, eventIds)
                    .Get();

                var eventsById = eventsResp.Models.ToDictionary(e => e.Id, e => e);

                foreach (var inv in invites)
                {
                    if (eventsById.TryGetValue(inv.EventId, out var ev))
                        inv.Event = ev;
                }
            }


            return View(invites);
        }

        // POST: /Invitations/Send
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(Guid eventId, string inviteTo, string invitedRole = "Guest")
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            var currentUserId = GetCurrentUserId()!.Value;
            var role = GetCurrentUserRole();

            if (string.IsNullOrWhiteSpace(inviteTo))
                return RedirectToAction("Details", "Events", new { id = eventId });

            // Only Admin or Organizer can invite
            if (role != "Admin" && role != "Organizer")
                return Forbid();

            inviteTo = inviteTo.Trim();
            var inviteLower = inviteTo.ToLowerInvariant();

            // Find invited user by username first, then email (avoid OR filter issues)
            User? invitedUser = null;

            var byUsername = await _supabase
                .From<User>()
                .Where(u => u.Username == inviteLower)
                .Limit(1)
                .Get();

            invitedUser = byUsername.Models.FirstOrDefault();

            if (invitedUser == null)
            {
                var byEmail = await _supabase
                    .From<User>()
                    .Where(u => u.Email == inviteLower)
                    .Limit(1)
                    .Get();

                invitedUser = byEmail.Models.FirstOrDefault();
            }

            // Prevent duplicates (we enforce also in DB, but catch early here)
            // Check duplicate by invited_user_id if user exists; else by invited_email
            if (invitedUser != null)
            {
                var dupResp = await _supabase
                    .From<Invitation>()
                    .Where(i => i.EventId == eventId)
                    .Where(i => i.InvitedUserId == invitedUser.Id)
                    .Limit(1)
                    .Get();

                if (dupResp.Models.Any())
                {
                    TempData["Error"] = "Invite already exists for this user.";
                    return RedirectToAction("Details", "Events", new { id = eventId });
                }
            }
            else
            {
                var dupResp = await _supabase
                    .From<Invitation>()
                    .Where(i => i.EventId == eventId)
                    .Where(i => i.InvitedEmail == inviteLower)
                    .Limit(1)
                    .Get();

                if (dupResp.Models.Any())
                {
                    TempData["Error"] = "Invite already exists for this email.";
                    return RedirectToAction("Details", "Events", new { id = eventId });
                }
            }

            var inv = new Invitation
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                InvitedByUserId = currentUserId,
                InvitedRole = string.IsNullOrWhiteSpace(invitedRole) ? "Guest" : invitedRole.Trim(),
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                RespondedAt = null,
                InvitedUserId = invitedUser?.Id,
                InvitedEmail = invitedUser != null
                    ? (invitedUser.Email ?? inviteLower).ToLowerInvariant()
                    : inviteLower
            };

            try
            {
                await _supabase.From<Invitation>().Insert(inv);
                TempData["Success"] = "Invitation sent.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error sending invite: {ex.Message}";
            }

            return RedirectToAction("Details", "Events", new { id = eventId });
        }

        // POST: /Invitations/Accept
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(Guid id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            var userId = GetCurrentUserId()!.Value;
            var me = await GetCurrentUserAsync();
            var myEmail = (me?.Email ?? "").Trim().ToLowerInvariant();

            var invResp = await _supabase
                .From<Invitation>()
                .Where(i => i.Id == id)
                .Limit(1)
                .Get();

            var inv = invResp.Models.FirstOrDefault();
            if (inv == null) return NotFound();

            var isMine =
                (inv.InvitedUserId != null && inv.InvitedUserId == userId) ||
                (!string.IsNullOrWhiteSpace(myEmail) && !string.IsNullOrWhiteSpace(inv.InvitedEmail) &&
                 inv.InvitedEmail.Trim().ToLowerInvariant() == myEmail);

            if (!isMine) return Forbid();
            if (inv.Status != "Pending") return RedirectToAction("My");

            // Update invitation
            inv.Status = "Accepted";
            inv.RespondedAt = DateTime.UtcNow;

            try
            {
                await _supabase.From<Invitation>().Update(inv);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating invitation: {ex.Message}";
                return RedirectToAction("My");
            }

            // Create participant if not exists
            try
            {
                var partCheck = await _supabase
                    .From<Participant>()
                    .Where(p => p.EventId == inv.EventId)
                    .Where(p => p.UserId == userId)
                    .Limit(1)
                    .Get();

                if (!partCheck.Models.Any())
                {
                    var participant = new Participant
                    {
                        Id = Guid.NewGuid(),
                        EventId = inv.EventId,
                        UserId = userId,
                        Role = inv.InvitedRole
                    };

                    await _supabase.From<Participant>().Insert(participant);
                }

                TempData["Success"] = "Invitation accepted.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Accepted, but participant insert failed: {ex.Message}";
            }

            return RedirectToAction("Details", "Events", new { id = inv.EventId });
        }

        // POST: /Invitations/Decline
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decline(Guid id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            var userId = GetCurrentUserId()!.Value;
            var me = await GetCurrentUserAsync();
            var myEmail = (me?.Email ?? "").Trim().ToLowerInvariant();

            var invResp = await _supabase
                .From<Invitation>()
                .Where(i => i.Id == id)
                .Limit(1)
                .Get();

            var inv = invResp.Models.FirstOrDefault();
            if (inv == null) return NotFound();

            var isMine =
                (inv.InvitedUserId != null && inv.InvitedUserId == userId) ||
                (!string.IsNullOrWhiteSpace(myEmail) && !string.IsNullOrWhiteSpace(inv.InvitedEmail) &&
                 inv.InvitedEmail.Trim().ToLowerInvariant() == myEmail);

            if (!isMine) return Forbid();
            if (inv.Status != "Pending") return RedirectToAction("My");

            inv.Status = "Declined";
            inv.RespondedAt = DateTime.UtcNow;

            try
            {
                await _supabase.From<Invitation>().Update(inv);
                TempData["Success"] = "Invitation declined.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error declining invitation: {ex.Message}";
            }

            return RedirectToAction("My");
        }

        // POST: /Invitations/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            var role = GetCurrentUserRole();
            var userId = GetCurrentUserId()!.Value;

            var invResp = await _supabase
                .From<Invitation>()
                .Where(i => i.Id == id)
                .Limit(1)
                .Get();

            var inv = invResp.Models.FirstOrDefault();
            if (inv == null) return NotFound();

            if (role != "Admin" && inv.InvitedByUserId != userId) return Forbid();
            if (inv.Status != "Pending") return RedirectToAction("Details", "Events", new { id = inv.EventId });

            inv.Status = "Cancelled";
            inv.RespondedAt = DateTime.UtcNow;

            try
            {
                await _supabase.From<Invitation>().Update(inv);
                TempData["Success"] = "Invitation cancelled.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error cancelling invitation: {ex.Message}";
            }

            return RedirectToAction("Details", "Events", new { id = inv.EventId });
        }
    }
}
