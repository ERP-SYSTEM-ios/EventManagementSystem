using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.Services;

namespace EventManagementSystem.Controllers
{
    public class LocationsController : Controller
    {
        private readonly SupabaseService _supabase;

        public LocationsController(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            var locations = await _supabase.GetLocations();
            return View(locations);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Create(string name, string? address, string? city, int? capacity)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ViewBag.Error = "Konum adı zorunludur.";
                return View();
            }

            await _supabase.AddLocation(name, address, city, capacity);
            return RedirectToAction(nameof(Index));
        }
    }
}
