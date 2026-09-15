using System;
using System.ComponentModel.DataAnnotations;

namespace AutoAlertBackEnd.Models
{
    public class Notifications : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AlertId { get; set; }
        public Guid UserId { get; set; }
        public DateTime? SentAt { get; set; }

        [MaxLength(150)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Message { get; set; }

        public bool IsRead { get; set; }

        [MaxLength(100)]
        public string? Result { get; set; }

        [MaxLength(50)]
        public string? Channel { get; set; }

        public Alerts? Alert { get; set; }
        public Users? User { get; set; }
    }
}
