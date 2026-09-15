using System;
using AutoAlertBackEnd.Context;
using AutoAlertBackEnd.Dtos;
using AutoAlertBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoAlertBackEnd.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AutoAlertContext _context;

    public RoleRepository(AutoAlertContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Roles>> GetAllRolesAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<Roles?> GetRoleByIdAsync(Guid id)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task<Roles> CreateRoleAsync(Roles role)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<Roles?> UpdateRoleAsync(Roles role)
    {
        var existingRole = await _context.Roles.FindAsync(role.Id);
        
        if (existingRole == null)
            return null;

        _context.Entry(existingRole).CurrentValues.SetValues(role);
        await _context.SaveChangesAsync();
        
        return existingRole;
    }

    public async Task<bool> DeleteRoleAsync(Guid id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null)
            return false;

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Roles?> GetRoleByNameAsync(string name)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == name);
    }


    public async Task<UserRolePermissionsDto> GetPermissionByUserAsync(Users user)
    {
        var roles = await _context.Roles.FirstOrDefaultAsync(r => r.Id == user.RoleId);

        var permissions = await _context.RoleSubModules
            .Where(rsm => rsm.RoleId == user.RoleId && rsm.IsEnabled && rsm.SubModule != null)
            .Select(rsm => rsm.SubModule!.Name)
            .ToListAsync();

        var overrides = await _context.UserSubmodules
            .Include(us => us.SubModule)
            .Where(us => us.UserId == user.Id)
            .ToListAsync();

        foreach (var item in overrides)
        {
            if (item.SubModule is null)
                continue;

            if (item.IsEnabled)
                permissions.Add(item.SubModule.Name);
            else
                permissions.Remove(item.SubModule.Name);
        }
    
        return new UserRolePermissionsDto
        {
            Role = roles?.Name ?? string.Empty,
            SpecialPermissions = permissions.Distinct().ToList(),
        };
    }
}
