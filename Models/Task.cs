using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace EventManagementSystem.Models
{
    [Table("tasks")]
    public class Task : BaseModel
    {

        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("event_id")]
        public Guid EventId { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("due_date")]
        public DateTime? DueDate { get; set; }

        [Column("priority")]
        public string Priority { get; set; } = "Medium";

        [Column("budget")]
        public decimal? Budget { get; set; }

        [Column("comment")]
        public string? Comment { get; set; }

        [Column("assigned_to")]
        public Guid? AssignedTo { get; set; }

        [Column("status")]
        public string Status { get; set; } = "Pending";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }


        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public User? AssignedUser { get; set; }
    }
}
