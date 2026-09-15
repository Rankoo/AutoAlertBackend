using Microsoft.Extensions.Caching.Memory;
using AutoAlertBackEnd.Context;
using Microsoft.EntityFrameworkCore;

public class PermissionService : IPermissionService
{
    private readonly IMemoryCache _cache;
    private readonly AutoAlertContext _context;

    public PermissionService(IMemoryCache cache, AutoAlertContext context)
    {
        _cache = cache;
        _context = context;
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permission)
    {
        if (userId == Guid.Empty)
            return false;
        if (string.IsNullOrEmpty(permission))
            return false;

        var cacheKey = $"permissions_{userId}";

        if (!_cache.TryGetValue(cacheKey, out List<string>? permissions))
        {
            permissions = await GetPermissionsFromDatabase(userId);

            _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(10));
        }

        return permissions!.Contains(permission);
    }

    private async Task<List<string>?> GetPermissionsFromDatabase(Guid userId)
    {
         var user = await _context.Users.FindAsync(userId);
        if (user == null)            return null;
        var permissions = await _context.RoleSubModules.Where(r=> r.RoleId == user.RoleId && r.SubModule != null).Select(r=> r.SubModule!.Name).ToListAsync();
        var overRides = await _context.UserSubmodules.Where(r => r.UserId == user.Id).ToListAsync();
        foreach (var overRideItem in overRides)
        {
            if (overRideItem.SubModule is null)
                continue;
            if (overRideItem.IsEnabled)
                permissions.Add(overRideItem.SubModule.Name);
            else
                permissions.Remove(overRideItem.SubModule.Name);
        }

        return [.. permissions.Distinct()];
    }
}