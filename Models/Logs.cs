using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AutoAlertBackEnd.Models
{
    public class Logs : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }

        [Required, MaxLength(100)]
        public required string Action { get; set; }

        [Required, MaxLength(100)]
        public required string TableName { get; set; }

        public Guid? RecordId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        [JsonIgnore]
        public Users? User { get; set; }
    }
}
