using AutoAlertBackEnd.Context;
using AutoAlertBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoAlertBackEnd.Repositories;

public class UserCompanyRepository : IUserCompanyRepository
{
    private readonly AutoAlertContext _context;

    public UserCompanyRepository(AutoAlertContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserCompanies>> GetAllAsync()
    {
        return await _context.UserCompanies.ToListAsync();
    }

    public async Task<UserCompanies?> GetByIdAsync(Guid userId, Guid companyId)
    {
        return await _context.UserCompanies
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CompanyId == companyId);
    }

    public async Task<IEnumerable<UserCompanies>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserCompanies
            .Where(uc => uc.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserCompanies>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.UserCompanies
            .Where(uc => uc.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<UserCompanies> CreateAsync(UserCompanies userCompany)
    {
        _context.UserCompanies.Add(userCompany);
        await _context.SaveChangesAsync();
        return userCompany;
    }

    public async Task<UserCompanies?> UpdateAsync(UserCompanies userCompany)
    {
        var existing = await _context.UserCompanies
            .FirstOrDefaultAsync(uc => uc.UserId == userCompany.UserId && uc.CompanyId == userCompany.CompanyId);
        
        if (existing == null) return null;
        _context.Entry(existing).CurrentValues.SetValues(userCompany);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid companyId)
    {
        var existing = await _context.UserCompanies
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CompanyId == companyId);
        
        if (existing == null) return false;
        _context.UserCompanies.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}

