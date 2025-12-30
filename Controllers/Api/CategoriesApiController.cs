using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.Services;

namespace EventManagementSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesApiController : ControllerBase
    {
        private readonly SupabaseService _supabase;

        public CategoriesApiController(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        // GET: /api/CategoriesApi
        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> GetAll()
        {
            var categories = await _supabase.GetCategories();

            // return only what UI needs
            var result = categories.Select(c => new
            {
                id = c.Id,
                name = c.Name,
                description = c.Description
            });

            return Ok(result);
        }

        // POST: /api/CategoriesApi
        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Create([FromBody] CreateCategoryRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                return BadRequest(new { message = "Name is required" });

            await _supabase.AddCategory(req.Name, req.Description);
            return Ok(new { message = "Category created" });
        }

        public class CreateCategoryRequest
        {
            public string Name { get; set; } = "";
            public string? Description { get; set; }
        }
    }
}
