using System;
using AutoAlertBackEnd.Dtos;
using AutoAlertBackEnd.Models;

namespace AutoAlertBackEnd.Repositories;

public interface IRoleRepository
{
    Task<IEnumerable<Roles>> GetAllRolesAsync();
    Task<Roles?> GetRoleByIdAsync(Guid id);
    Task<Roles> CreateRoleAsync(Roles role);
    Task<Roles?> UpdateRoleAsync(Roles role);
    Task<bool> DeleteRoleAsync(Guid id);
    Task<Roles?> GetRoleByNameAsync(string name);
    Task<UserRolePermissionsDto> GetPermissionByUserAsync(Users user);
}