using AutoAlertBackEnd.Context;
using AutoAlertBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoAlertBackEnd.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly AutoAlertContext _context;

    public CompanyRepository(AutoAlertContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Companies>> GetAllAsync()
    {
        return await _context.Companies.ToListAsync();
    }

    public async Task<Companies?> GetByIdAsync(Guid id)
    {
        return await _context.Companies.FindAsync(id);
    }

    public async Task<Companies> CreateAsync(Companies company)
    {
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();
        return company;
    }

    public async Task<Companies?> UpdateAsync(Companies company)
    {
        var existing = await _context.Companies.FindAsync(company.Id);
        if (existing == null) return null;
        _context.Entry(existing).CurrentValues.SetValues(company);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.Companies.FindAsync(id);
        if (existing == null) return false;
        _context.Companies.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
