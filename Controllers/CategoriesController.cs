using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.Models;
using EventManagementSystem.Services;

namespace EventManagementSystem.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly SupabaseService _supabase;

        public CategoriesController(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        // GET: /Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _supabase.GetCategories();
            return View(categories);
        }

        // GET: /Categories/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Categories/Create
        [HttpPost]
        public async Task<IActionResult> Create(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ViewBag.Error = "Kategori adı zorunludur.";
                return View();
            }

            await _supabase.AddCategory(name, description);
            return RedirectToAction(nameof(Index));
        }
    }
}
