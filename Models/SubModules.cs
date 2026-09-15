using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoAlertBackEnd.Models
{
    public class SubModules
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ModuleId { get; set; }
        [Required, MaxLength(100)]
        public required string Name { get; set; }

        public Modules? Module { get; set; }
        public ICollection<RoleSubModules>? RoleSubModules { get; set; }
        public ICollection<UserSubmodules>? UserSubmodules { get; set; }
    }
}
