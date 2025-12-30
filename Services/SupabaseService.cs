using Supabase;
using Microsoft.Extensions.Configuration;
using EventManagementSystem.Models;

namespace EventManagementSystem.Services
{
    public class SupabaseService
    {
        public Supabase.Client Client { get; }

        public SupabaseService(IConfiguration configuration)
        {
            var url = configuration["Supabase:Url"];
            var key = configuration["Supabase:Key"];

            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key))
                throw new Exception("Supabase:Url or Supabase:Key are missing in appsettings.json");

            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = false
            };

            Client = new Supabase.Client(url, key, options);
            Client.InitializeAsync().Wait();
        }

        // =========================
        // CATEGORIES
        // =========================

        public async System.Threading.Tasks.Task<List<Category>> GetCategories()
        {
            var response = await Client
                .From<Category>()
                .Select("*")
                .Order("name", Supabase.Postgrest.Constants.Ordering.Ascending)
                .Get();

            return response.Models;
        }

        public async System.Threading.Tasks.Task AddCategory(string name, string? description)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            await Client.From<Category>().Insert(category);
        }

        public async System.Threading.Tasks.Task<List<Location>> GetLocations()
        {
            var response = await Client
                .From<Location>()
                .Select("*")
                .Order("name", Supabase.Postgrest.Constants.Ordering.Ascending)
                .Get();

            return response.Models;
        }

        public async System.Threading.Tasks.Task AddLocation(string name, string? address, string? city, int? capacity)
        {
            var location = new Location
            {
                Id = Guid.NewGuid(),
                Name = name,
                Address = address,
                City = city,
                Capacity = capacity,
                CreatedAt = DateTime.UtcNow
            };

            await Client.From<Location>().Insert(location);
        }

        public async System.Threading.Tasks.Task AddEvent(Event ev)
        {
            // basic sanity
            if (ev.Id == Guid.Empty) ev.Id = Guid.NewGuid();
            if (ev.CreatedAt == default) ev.CreatedAt = DateTime.UtcNow;

            await Client.From<Event>().Insert(ev);
        }

        public async System.Threading.Tasks.Task<Dictionary<Guid, string>> GetCategoryNameMap()
{
    var categories = await GetCategories();
    return categories.ToDictionary(c => c.Id, c => c.Name);
}

public async System.Threading.Tasks.Task<Dictionary<Guid, string>> GetLocationNameMap()
{
    var locations = await GetLocations();
    return locations.ToDictionary(
        l => l.Id,
        l => $"{l.Name}{(string.IsNullOrWhiteSpace(l.City) ? "" : " - " + l.City)}"
    );
}




    }
}
