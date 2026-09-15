using AutoAlertBackEnd.Context;
using AutoAlertBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoAlertBackEnd.Repositories;

public class RoleSubModuleRepository : IRoleSubModuleRepository
{
    private readonly AutoAlertContext _context;

    public RoleSubModuleRepository(AutoAlertContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoleSubModules>> GetAllAsync()
    {
        return await _context.RoleSubModules.ToListAsync();
    }

    public async Task<RoleSubModules?> GetByIdAsync(Guid id)
    {
        return await _context.RoleSubModules.FindAsync(id);
    }

    public async Task<IEnumerable<RoleSubModules>> GetByRoleIdAsync(Guid roleId)
    {
        return await _context.RoleSubModules
            .Where(rsm => rsm.RoleId == roleId)
            .ToListAsync();
    }

    public async Task<IEnumerable<RoleSubModules>> GetBySubModuleIdAsync(Guid subModuleId)
    {
        return await _context.RoleSubModules
            .Where(rsm => rsm.SubModuleId == subModuleId)
            .ToListAsync();
    }

    public async Task<RoleSubModules> CreateAsync(RoleSubModules roleSubModule)
    {
        _context.RoleSubModules.Add(roleSubModule);
        await _context.SaveChangesAsync();
        return roleSubModule;
    }

    public async Task<RoleSubModules?> UpdateAsync(RoleSubModules roleSubModule)
    {
        var existing = await _context.RoleSubModules.FindAsync(roleSubModule.Id);
        if (existing == null) return null;
        _context.Entry(existing).CurrentValues.SetValues(roleSubModule);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.RoleSubModules.FindAsync(id);
        if (existing == null) return false;
        _context.RoleSubModules.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}

