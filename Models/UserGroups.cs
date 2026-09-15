using System;
using System.ComponentModel.DataAnnotations;

namespace AutoAlertBackEnd.Models
{
    public class UserGroups : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }

        [MaxLength(50)]
        public string? AccessType { get; set; } = "view";

        

        public Users? User { get; set; }
        public Groups? Group { get; set; }
    }
}
