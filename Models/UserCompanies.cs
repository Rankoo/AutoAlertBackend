using System;
using System.ComponentModel.DataAnnotations;

namespace AutoAlertBackEnd.Models
{
    public class UserCompanies : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }

        [MaxLength(50)]
        public string? AccessType { get; set; } = "view";

        

        public Users? User { get; set; }
        public Companies? Company { get; set; }
    }
}
