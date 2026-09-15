using AutoAlertBackEnd.Models;

namespace AutoAlertBackEnd.Repositories;

public interface IRoleSubModuleRepository
{
    Task<IEnumerable<RoleSubModules>> GetAllAsync();
    Task<RoleSubModules?> GetByIdAsync(Guid id);
    Task<IEnumerable<RoleSubModules>> GetByRoleIdAsync(Guid roleId);
    Task<IEnumerable<RoleSubModules>> GetBySubModuleIdAsync(Guid subModuleId);
    Task<RoleSubModules> CreateAsync(RoleSubModules roleSubModule);
    Task<RoleSubModules?> UpdateAsync(RoleSubModules roleSubModule);
    Task<bool> DeleteAsync(Guid id);
}

