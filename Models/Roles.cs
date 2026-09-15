using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoAlertBackEnd.Models
{
    public class Roles : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }
        

        public ICollection<Users>? Users { get; set; }
        public ICollection<RoleSubModules>? RoleSubModules { get; set; }
    }
}
