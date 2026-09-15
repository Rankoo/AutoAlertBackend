using AutoAlertBackEnd.Context;
using AutoAlertBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoAlertBackEnd.Repositories;

public class UserSubmoduleRepository : IUserSubmoduleRepository
{
    private readonly AutoAlertContext _context;

    public UserSubmoduleRepository(AutoAlertContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserSubmodules>> GetAllAsync()
    {
        return await _context.UserSubmodules.ToListAsync();
    }

    public async Task<UserSubmodules?> GetByIdAsync(Guid userId, Guid subModuleId)
    {
        return await _context.UserSubmodules
            .FirstOrDefaultAsync(us => us.UserId == userId && us.SubModuleId == subModuleId);
    }

    public async Task<IEnumerable<UserSubmodules>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserSubmodules
            .Where(us => us.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserSubmodules>> GetBySubModuleIdAsync(Guid subModuleId)
    {
        return await _context.UserSubmodules
            .Where(us => us.SubModuleId == subModuleId)
            .ToListAsync();
    }

    public async Task<UserSubmodules> CreateAsync(UserSubmodules userSubmodule)
    {
        _context.UserSubmodules.Add(userSubmodule);
        await _context.SaveChangesAsync();
        return userSubmodule;
    }

    public async Task<UserSubmodules?> UpdateAsync(UserSubmodules userSubmodule)
    {
        var existing = await _context.UserSubmodules
            .FirstOrDefaultAsync(us => us.UserId == userSubmodule.UserId && us.SubModuleId == userSubmodule.SubModuleId);
        
        if (existing == null) return null;
        _context.Entry(existing).CurrentValues.SetValues(userSubmodule);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid subModuleId)
    {
        var existing = await _context.UserSubmodules
            .FirstOrDefaultAsync(us => us.UserId == userId && us.SubModuleId == subModuleId);
        
        if (existing == null) return false;
        _context.UserSubmodules.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}

