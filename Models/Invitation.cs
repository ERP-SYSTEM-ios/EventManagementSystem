using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Supabase.Postgrest.Models;
using PostgrestColumn = Supabase.Postgrest.Attributes.ColumnAttribute;
using PostgrestPrimaryKey = Supabase.Postgrest.Attributes.PrimaryKeyAttribute;
using PostgrestTable = Supabase.Postgrest.Attributes.TableAttribute;

namespace EventManagementSystem.Models
{
    [Table("invitations")]
    [PostgrestTable("invitations")]
    public class Invitation : BaseModel
    {
        [Key]
        [Column("id")]
        [PostgrestPrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Required]
        [Column("event_id")]
        [PostgrestColumn("event_id")]
        public Guid EventId { get; set; }

        [Column("invited_user_id")]
        [PostgrestColumn("invited_user_id")]
        public Guid? InvitedUserId { get; set; }

        [Column("invited_email")]
        [StringLength(100)]
        [PostgrestColumn("invited_email")]
        public string? InvitedEmail { get; set; }

        [Required]
        [Column("invited_by_user_id")]
        [PostgrestColumn("invited_by_user_id")]
        public Guid InvitedByUserId { get; set; }

        [Required]
        [Column("invited_role")]
        [StringLength(20)]
        [PostgrestColumn("invited_role")]
        public string InvitedRole { get; set; } = "Guest";

        [Required]
        [Column("status")]
        [StringLength(20)]
        [PostgrestColumn("status")]
        public string Status { get; set; } = "Pending";

        [Column("created_at")]
        [PostgrestColumn("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("responded_at")]
        [PostgrestColumn("responded_at")]
        public DateTime? RespondedAt { get; set; }

        [ForeignKey(nameof(EventId))]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Event? Event { get; set; }

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Email => InvitedEmail ?? string.Empty;

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string Role => InvitedRole;

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime SentAt => CreatedAt;
    }
}
