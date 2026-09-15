using AutoAlertBackEnd.Context;
using AutoAlertBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoAlertBackEnd.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly AutoAlertContext _context;

    public GroupRepository(AutoAlertContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Groups>> GetAllAsync()
    {
        return await _context.Groups.ToListAsync();
    }

    public async Task<Groups?> GetByIdAsync(Guid id)
    {
        return await _context.Groups.FindAsync(id);
    }

    public async Task<Groups> CreateAsync(Groups group)
    {
        _context.Groups.Add(group);
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task<Groups?> UpdateAsync(Groups group)
    {
        var existing = await _context.Groups.FindAsync(group.Id);
        if (existing == null) return null;
        _context.Entry(existing).CurrentValues.SetValues(group);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.Groups.FindAsync(id);
        if (existing == null) return false;
        _context.Groups.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
