using AutoAlertBackEnd.Models;

namespace AutoAlertBackEnd.Repositories;

public interface IGroupRepository
{
    Task<IEnumerable<Groups>> GetAllAsync();
    Task<Groups?> GetByIdAsync(Guid id);
    Task<Groups> CreateAsync(Groups group);
    Task<Groups?> UpdateAsync(Groups group);
    Task<bool> DeleteAsync(Guid id);
}
