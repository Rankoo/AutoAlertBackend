using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AutoAlertBackEnd.Models
{
    public class Users : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid DocumentTypeId { get; set; }
        public Guid RoleId { get; set; }

        [Required, MaxLength(150)]
        public required string Names { get; set; }

        [MaxLength(150)]
        public string? LastNames { get; set; }

        [Required, EmailAddress, MaxLength(150)]
        public required string Email { get; set; }

        [MaxLength(255)]
        public string? PasswordHash { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(255)]
        public string? GoogleId { get; set; }

        [MaxLength(50)]
        public string? DocumentNumber { get; set; }
        public string? Position { get; set; }
        public bool IsActive { get; set; } = true;
        public bool ChangePassword { get; set; } = true;
        public DateTimeOffset? LastLoginAt { get; set; }

        // Navigation properties
        [JsonIgnore]
        public DocumentTypes? DocumentType { get; set; }
        
        [JsonIgnore]
        public Roles? Role { get; set; }

        [JsonIgnore]
        public ICollection<Notifications>? Notifications { get; set; }
        
        [JsonIgnore]
        public ICollection<UserCompanies>? UserCompanies { get; set; }
        
        [JsonIgnore]
        public ICollection<UserGroups>? UserGroups { get; set; }
        
        [JsonIgnore]
        public ICollection<UserSubmodules>? UserSubmodules { get; set; }
    }
}
