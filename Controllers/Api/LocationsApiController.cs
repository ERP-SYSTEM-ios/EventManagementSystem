using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.Services;

namespace EventManagementSystem.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsApiController : ControllerBase
    {
        private readonly SupabaseService _supabase;

        public LocationsApiController(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> GetAll()
        {
            var locations = await _supabase.GetLocations();

            var result = locations.Select(l => new
            {
                id = l.Id,
                name = l.Name,
                city = l.City,
                address = l.Address,
                capacity = l.Capacity
            });

            return Ok(result);
        }
    }
}
