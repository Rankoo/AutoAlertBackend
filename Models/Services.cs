using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoAlertBackEnd.Models
{
    public class Services : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid StoreId { get; set; }
    [Required, MaxLength(150)]
    public required string Name { get; set; }

    [MaxLength(100)]
    public string? Provider { get; set; }

    [MaxLength(100)]
    public string? AccountNumber { get; set; }
        public Stores? Store { get; set; }
        public ICollection<Alerts>? Alerts { get; set; }
    }
}
