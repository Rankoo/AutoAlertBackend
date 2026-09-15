using AutoAlertBackEnd.Models;

namespace AutoAlertBackEnd.Repositories;

public interface IModuleRepository
{
    Task<IEnumerable<Modules>> GetAllAsync();
    Task<Modules?> GetByIdAsync(Guid id);
    Task<Modules> CreateAsync(Modules module);
    Task<Modules?> UpdateAsync(Modules module);
    Task<bool> DeleteAsync(Guid id);
}

