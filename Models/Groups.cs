using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AutoAlertBackEnd.Models
{
    public class Groups : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required, MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        [JsonIgnore]
        public ICollection<Companies>? Companies { get; set; }

        [JsonIgnore]
        public ICollection<UserGroups>? UserGroups { get; set; }
    }
}
