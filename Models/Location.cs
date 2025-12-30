using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace EventManagementSystem.Models
{
    [Table("locations")]
    public class Location : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("address")]
        public string? Address { get; set; }

        [Column("city")]
        public string? City { get; set; }

        [Column("capacity")]
        public int? Capacity { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
