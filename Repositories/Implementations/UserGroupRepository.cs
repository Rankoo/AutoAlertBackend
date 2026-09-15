using AutoAlertBackEnd.Context;
using AutoAlertBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoAlertBackEnd.Repositories;

public class UserGroupRepository : IUserGroupRepository
{
    private readonly AutoAlertContext _context;

    public UserGroupRepository(AutoAlertContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserGroups>> GetAllAsync()
    {
        return await _context.UserGroups.ToListAsync();
    }

    public async Task<UserGroups?> GetByIdAsync(Guid userId, Guid groupId)
    {
        return await _context.UserGroups
            .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GroupId == groupId);
    }

    public async Task<IEnumerable<UserGroups>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserGroups
            .Where(ug => ug.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserGroups>> GetByGroupIdAsync(Guid groupId)
    {
        return await _context.UserGroups
            .Where(ug => ug.GroupId == groupId)
            .ToListAsync();
    }

    public async Task<UserGroups> CreateAsync(UserGroups userGroup)
    {
        _context.UserGroups.Add(userGroup);
        await _context.SaveChangesAsync();
        return userGroup;
    }

    public async Task<UserGroups?> UpdateAsync(UserGroups userGroup)
    {
        var existing = await _context.UserGroups
            .FirstOrDefaultAsync(ug => ug.UserId == userGroup.UserId && ug.GroupId == userGroup.GroupId);
        
        if (existing == null) return null;
        _context.Entry(existing).CurrentValues.SetValues(userGroup);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid groupId)
    {
        var existing = await _context.UserGroups
            .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GroupId == groupId);
        
        if (existing == null) return false;
        _context.UserGroups.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}

