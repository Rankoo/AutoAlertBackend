using AutoAlertBackEnd.Context;
using AutoAlertBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoAlertBackEnd.Repositories;

public class ModuleRepository : IModuleRepository
{
    private readonly AutoAlertContext _context;

    public ModuleRepository(AutoAlertContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Modules>> GetAllAsync()
    {
        return await _context.Modules.ToListAsync();
    }

    public async Task<Modules?> GetByIdAsync(Guid id)
    {
        return await _context.Modules.FindAsync(id);
    }

    public async Task<Modules> CreateAsync(Modules module)
    {
        _context.Modules.Add(module);
        await _context.SaveChangesAsync();
        return module;
    }

    public async Task<Modules?> UpdateAsync(Modules module)
    {
        var existing = await _context.Modules.FindAsync(module.Id);
        if (existing == null) return null;
        _context.Entry(existing).CurrentValues.SetValues(module);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.Modules.FindAsync(id);
        if (existing == null) return false;
        _context.Modules.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}

