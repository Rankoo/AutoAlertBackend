using AutoAlertBackEnd.Context;
using AutoAlertBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoAlertBackEnd.Repositories;

public class SubModuleRepository : ISubModuleRepository
{
    private readonly AutoAlertContext _context;

    public SubModuleRepository(AutoAlertContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SubModules>> GetAllAsync()
    {
        return await _context.SubModules.ToListAsync();
    }

    public async Task<SubModules?> GetByIdAsync(Guid id)
    {
        return await _context.SubModules.FindAsync(id);
    }

    public async Task<IEnumerable<SubModules>> GetByModuleIdAsync(Guid moduleId)
    {
        return await _context.SubModules
            .Where(sm => sm.ModuleId == moduleId)
            .ToListAsync();
    }

    public async Task<SubModules> CreateAsync(SubModules subModule)
    {
        _context.SubModules.Add(subModule);
        await _context.SaveChangesAsync();
        return subModule;
    }

    public async Task<SubModules?> UpdateAsync(SubModules subModule)
    {
        var existing = await _context.SubModules.FindAsync(subModule.Id);
        if (existing == null) return null;
        _context.Entry(existing).CurrentValues.SetValues(subModule);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.SubModules.FindAsync(id);
        if (existing == null) return false;
        _context.SubModules.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}

