using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace EventManagementSystem.Models
{
    [Table("participants")]
    public class Participant : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("event_id")]
        public Guid EventId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("role")]
        public string? Role { get; set; }

        [Column("can_edit")]
        public bool CanEdit { get; set; }

        [Column("invited_at")]
        public DateTime InvitedAt { get; set; }

   

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Event? Event { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public User? User { get; set; }
    }
}
