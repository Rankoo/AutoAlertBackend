using AutoAlertBackEnd.Models;

namespace AutoAlertBackEnd.Repositories;

public interface IUserCompanyRepository
{
    Task<IEnumerable<UserCompanies>> GetAllAsync();
    Task<UserCompanies?> GetByIdAsync(Guid userId, Guid companyId);
    Task<IEnumerable<UserCompanies>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<UserCompanies>> GetByCompanyIdAsync(Guid companyId);
    Task<UserCompanies> CreateAsync(UserCompanies userCompany);
    Task<UserCompanies?> UpdateAsync(UserCompanies userCompany);
    Task<bool> DeleteAsync(Guid userId, Guid companyId);
}

