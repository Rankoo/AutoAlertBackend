using AutoAlertBackEnd.Models;

namespace AutoAlertBackEnd.Repositories;

public interface ICompanyRepository
{
    Task<IEnumerable<Companies>> GetAllAsync();
    Task<Companies?> GetByIdAsync(Guid id);
    Task<Companies> CreateAsync(Companies company);
    Task<Companies?> UpdateAsync(Companies company);
    Task<bool> DeleteAsync(Guid id);
}
