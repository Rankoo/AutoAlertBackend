using AutoAlertBackEnd.Models;

namespace AutoAlertBackEnd.Repositories;

public interface IUserSubmoduleRepository
{
    Task<IEnumerable<UserSubmodules>> GetAllAsync();
    Task<UserSubmodules?> GetByIdAsync(Guid userId, Guid subModuleId);
    Task<IEnumerable<UserSubmodules>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<UserSubmodules>> GetBySubModuleIdAsync(Guid subModuleId);
    Task<UserSubmodules> CreateAsync(UserSubmodules userSubmodule);
    Task<UserSubmodules?> UpdateAsync(UserSubmodules userSubmodule);
    Task<bool> DeleteAsync(Guid userId, Guid subModuleId);
}

