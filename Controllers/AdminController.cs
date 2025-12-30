using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Supabase;
using static Supabase.Postgrest.Constants;
using EventManagementSystem.Models;
using EventManagementSystem.Services;

namespace EventManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly Client _supabase;

        public AdminController(SupabaseService supabaseService)
        {
            _supabase = supabaseService.Client;
        }


        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetString("UserId") != null;
        }

        private Guid? GetCurrentUserId()
        {
            var idStr = HttpContext.Session.GetString("UserId");
            if (Guid.TryParse(idStr, out var id))
                return id;
            return null;
        }

        private string GetCurrentUserRole()
        {
            return HttpContext.Session.GetString("Role") ?? "";
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Auth");

            var role = GetCurrentUserRole();
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                return Forbid();

            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewBag.UserRole = role;

            try
            {
                var usersResp = await _supabase
                    .From<User>()
                    .Order(u => u.CreatedAt, Ordering.Descending)
                    .Get();
                var users = usersResp.Models;

                var totalUsers = users.Count;
                var totalAdmins = users.Count(u =>
                    string.Equals(u.Role, "Admin", StringComparison.OrdinalIgnoreCase));
                var totalOrganizers = users.Count(u =>
                    string.Equals(u.Role, "Organizer", StringComparison.OrdinalIgnoreCase));
                var totalGuests = users.Count(u =>
                    string.Equals(u.Role, "Guest", StringComparison.OrdinalIgnoreCase));

                // ----- Events -----
                var eventsResp = await _supabase
                    .From<Event>()
                    .Order(e => e.CreatedAt, Ordering.Descending)
                    .Get();
                var events = eventsResp.Models;

                var totalEvents = events.Count;
                var upcomingEvents = events.Count(e =>
                    string.Equals(e.Status, "Upcoming", StringComparison.OrdinalIgnoreCase));
                var ongoingEvents = events.Count(e =>
                    string.Equals(e.Status, "Ongoing", StringComparison.OrdinalIgnoreCase));
                var completedEvents = events.Count(e =>
                    string.Equals(e.Status, "Completed", StringComparison.OrdinalIgnoreCase));

                // ----- Tasks -----
                var tasksResp = await _supabase
                    .From<Models.Task>()
                    .Order(t => t.CreatedAt, Ordering.Descending)
                    .Get();
                var tasks = tasksResp.Models;

                var totalTasks = tasks.Count;
                var pendingTasks = tasks.Count(t =>
                    string.Equals(t.Status, "Pending", StringComparison.OrdinalIgnoreCase));
                var inProgressTasks = tasks.Count(t =>
                    string.Equals(t.Status, "In Progress", StringComparison.OrdinalIgnoreCase));
                var doneTasks = tasks.Count(t =>
                    string.Equals(t.Status, "Done", StringComparison.OrdinalIgnoreCase));

                // recent lists
                var recentUsers = users.Take(5).ToList();
                var recentEvents = events.Take(5).ToList();
                var recentTasks = tasks.Take(5).ToList();

                ViewBag.TotalUsers = totalUsers;
                ViewBag.TotalAdmins = totalAdmins;
                ViewBag.TotalOrganizers = totalOrganizers;
                ViewBag.TotalGuests = totalGuests;

                ViewBag.TotalEvents = totalEvents;
                ViewBag.UpcomingEvents = upcomingEvents;
                ViewBag.OngoingEvents = ongoingEvents;
                ViewBag.CompletedEvents = completedEvents;

                ViewBag.TotalTasks = totalTasks;
                ViewBag.PendingTasks = pendingTasks;
                ViewBag.InProgressTasks = inProgressTasks;
                ViewBag.DoneTasks = doneTasks;

                ViewBag.RecentUsers = recentUsers;
                ViewBag.RecentEvents = recentEvents;
                ViewBag.RecentTasks = recentTasks;
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading dashboard: {ex.Message}";
            }

            return View();
        }
    }
}
