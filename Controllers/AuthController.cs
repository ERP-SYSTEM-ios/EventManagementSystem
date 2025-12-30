using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Supabase;
using EventManagementSystem.Models;
using EventManagementSystem.Services;

namespace EventManagementSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly Client _supabase;

        public AuthController(SupabaseService supabaseService)
        {
            _supabase = supabaseService.Client;
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                // уже залогинен – отправляем куда тебе надо
                return RedirectToAction("Index", "Events");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Username and password are required.";
                return View();
            }

            try
            {
                var response = await _supabase
                    .From<User>()
                    .Where(u => u.Username == username && u.Password == password)
                    .Limit(1)
                    .Get();

                var user = response.Models.FirstOrDefault();

                if (user == null)
                {
                    ViewBag.Error = "Invalid username or password.";
                    return View();
                }

                // Store in session
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role);

                // Redirect based on role
                if (string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Dashboard", "Admin");
                }

                return RedirectToAction("Index", "Events");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Supabase error: {ex.Message}";
                return View();
            }
        }

        // GET: /Auth/Signup
        [HttpGet]
        public IActionResult Signup()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Events");
            }
            return View();
        }

        // POST: /Auth/Signup
        [HttpPost]
        public async Task<IActionResult> Signup(string username, string email, string password, string role = "Guest")
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }

            try
            {
                // check if user exists
                var existing = await _supabase
                    .From<User>()
                    .Where(u => u.Username == username || u.Email == email)
                    .Get();

                if (existing.Models.Any())
                {
                    ViewBag.Error = "Username or email already exists.";
                    return View();
                }

                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    Email = email,
                    Password = password, // да, у тебя plain text. Потом уже сделаем хэш.
                    Role = string.IsNullOrWhiteSpace(role) ? "Guest" : role,
                    CreatedAt = DateTime.UtcNow
                };

                await _supabase
                    .From<User>()
                    .Insert(newUser);

                // можно либо сразу логинить, либо отправить на Login
                // сейчас – отправляем на Login с сообщением
                TempData["Success"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Supabase error: {ex.Message}";
                return View();
            }
        }

        // GET: /Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: /Auth/UsersListTest
        // простой тест, чтобы увидеть, что SDK читает таблицу users
        [HttpGet]
        public async Task<IActionResult> UsersListTest()
        {
            try
            {
                var response = await _supabase
                    .From<User>()
                    .Limit(50)
                    .Get();

                var users = response.Models
                    .Select(u => $"{u.Username} | {u.Email} | {u.Role}")
                    .ToList();

                if (!users.Any())
                    return Content("No users found in Supabase.");

                return Content("Users:\n" + string.Join("\n", users));
            }
            catch (Exception ex)
            {
                return Content($"Supabase ERROR: {ex.GetType().Name} - {ex.Message}");
            }
        }
    }
}
