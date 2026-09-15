using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoAlertBackEnd.Models
{
    public class Stores : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? CompanyId { get; set; }
    [Required, MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(150)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

        public Companies? Company { get; set; }
        public ICollection<Services>? Services { get; set; }
    }
}
