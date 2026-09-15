using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoAlertBackEnd.Models
{
    public class Modules : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    [Required, MaxLength(100)]
    public required string Name { get; set; }
        

        public ICollection<SubModules>? SubModules { get; set; }
    }
}
