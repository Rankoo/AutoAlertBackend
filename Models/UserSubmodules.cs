using System;
using System.ComponentModel.DataAnnotations;

namespace AutoAlertBackEnd.Models
{
    public class UserSubmodules : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid SubModuleId { get; set; }

        public bool IsEnabled { get; set; }

        [Required]
        required public Users User { get; set; }
        
        [Required]
        required public SubModules SubModule { get; set; }
    }
}
