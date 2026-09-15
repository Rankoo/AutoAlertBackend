using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AutoAlertBackEnd.Models
{
    public class DocumentTypes : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required, MaxLength(50)]
        public required string Name { get; set; }

    [MaxLength(10)]
    public string? Abbreviation { get; set; }

        [JsonIgnore]
        public ICollection<Users>? Users { get; set; }
    }
}
