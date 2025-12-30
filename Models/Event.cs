using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace EventManagementSystem.Models
{
    [Table("events")]
    public class Event : BaseModel
    {
        // === Real DB columns ===

        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("category")]
        public string? Category { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("location")]
        public string? Location { get; set; }

        [Column("status")]
        public string Status { get; set; } = string.Empty;

        [Column("budget")]
        public decimal? Budget { get; set; }

        [Column("organizer_id")]
        public Guid OrganizerId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("location_id")]
        public Guid? LocationId { get; set; }

        [Column("category_id")]
        public Guid? CategoryId { get; set; }

        
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public User? Organizer { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<Participant>? Participants { get; set; }

    }
}
