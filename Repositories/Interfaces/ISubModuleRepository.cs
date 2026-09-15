using AutoAlertBackEnd.Models;

namespace AutoAlertBackEnd.Repositories;

public interface ISubModuleRepository
{
    Task<IEnumerable<SubModules>> GetAllAsync();
    Task<SubModules?> GetByIdAsync(Guid id);
    Task<IEnumerable<SubModules>> GetByModuleIdAsync(Guid moduleId);
    Task<SubModules> CreateAsync(SubModules subModule);
    Task<SubModules?> UpdateAsync(SubModules subModule);
    Task<bool> DeleteAsync(Guid id);
}

