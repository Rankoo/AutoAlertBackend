using AutoAlertBackEnd.Context;
using AutoAlertBackEnd.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AutoAlertBackEnd.Repositories;

public class UserCatalogRepository : IUserCatalogRepository
{
    private readonly AutoAlertContext _context;

    public UserCatalogRepository(AutoAlertContext context)
    {
        _context = context;
    }

    public async Task<UserCatalogsDto> GetUserCatalogsAsync()
    {
        var roles = await _context.Roles
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => new CatalogItemDto
            {
                Id = r.Id,
                Name = r.Name
            })
            .ToListAsync();

        var permissions = await _context.RoleSubModules
            .AsNoTracking()
            .OrderBy(rsm => rsm.Role!.Name)
            .ThenBy(rsm => rsm.SubModule!.Name)
            .Select(rsm => new PermissionCatalogDto
            {
                Id = rsm.Id,
                RoleId = rsm.RoleId,
                SubModuleId = rsm.SubModuleId,
                RoleName = rsm.Role!.Name,
                PermissionName = rsm.SubModule!.Name,
                IsEnabled = rsm.IsEnabled
            })
            .ToListAsync();

        var documentTypes = await _context.DocumentTypes
            .AsNoTracking()
            .OrderBy(dt => dt.Name)
            .Select(dt => new DocumentTypeCatalogDto
            {
                Id = dt.Id,
                Name = dt.Name,
                Abbreviation = dt.Abbreviation
            })
            .ToListAsync();

        return new UserCatalogsDto
        {
            Roles = roles,
            Permissions = permissions,
            DocumentTypes = documentTypes
        };
    }
}