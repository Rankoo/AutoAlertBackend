using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AutoAlertBackEnd.Models
{
    public class Companies : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? GroupId { get; set; }

        [Required, MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(20)]
        public string? NIT { get; set; }

        [MaxLength(150)]
        public string? Address { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
        public Groups? Group { get; set; }

        [JsonIgnore]
        public ICollection<Stores>? Stores { get; set; }
        
        [JsonIgnore]
        public ICollection<UserCompanies>? UserCompanies { get; set; }
    }
}
