using System;

namespace AutoAlertBackEnd.Models
{
    public class RoleSubModules : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RoleId { get; set; }
        public Guid SubModuleId { get; set; }
        public bool IsEnabled { get; set; }
        

        public Roles? Role { get; set; }
        public SubModules? SubModule { get; set; }
    }
}
