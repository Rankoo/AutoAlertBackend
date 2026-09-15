using AutoAlertBackEnd.Context;
using AutoAlertBackEnd.Dtos;
using AutoAlertBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoAlertBackEnd.Repositories;

public class StoreRepository : IStoreRepository
{
    private readonly AutoAlertContext _context;

    public StoreRepository(AutoAlertContext context)
    {
        _context = context;
    }

    public async Task<PagedStoresDto> GetAllAsync(int page, int pageSize, string? search = null)
    {
        var storesQuery = _context.Stores.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = $"%{search.Trim()}%";
            storesQuery = storesQuery.Where(s =>
                EF.Functions.Like(s.Name, searchTerm) ||
                (s.Address != null && EF.Functions.Like(s.Address, searchTerm)) ||
                (s.City != null && EF.Functions.Like(s.City, searchTerm)));
        }

        var projectedStores = storesQuery
            .OrderBy(s => s.Name)
            .Select(s => new StoreListDto
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address,
                City = s.City
            });

        var totalItems = await projectedStores.CountAsync();
        var stores = await projectedStores
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedStoresDto
        {
            Stores = stores,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public async Task<StoreDto?> GetByIdAsync(Guid id)
    {
        return await _context.Stores
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new StoreDto
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address,
                City = s.City,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<StoreQuantitiesDto> GetQuantitiesAsync()
    {
        var now = DateTime.Now;
        var sevenDaysAgo = now.AddDays(-7);

        return new StoreQuantitiesDto
        {
            TotalStores = await _context.Stores.CountAsync(),
            StoresWithServices = await _context.Stores.CountAsync(s => s.Services!.Any()),
            CitiesCount = await _context.Stores
                .Where(s => s.City != null && s.City != "")
                .Select(s => s.City)
                .Distinct()
                .CountAsync(),
            StoresCreatedLastSevenDays = await _context.Stores
                .CountAsync(s => s.CreatedAt >= sevenDaysAgo && s.CreatedAt <= now)
        };
    }

    public async Task<StoreDto> CreateAsync(CreateStoreDto store)
    {
        var entity = new Stores
        {
            Name = store.Name.Trim(),
            Address = string.IsNullOrWhiteSpace(store.Address) ? null : store.Address.Trim(),
            City = string.IsNullOrWhiteSpace(store.City) ? null : store.City.Trim()
        };

        _context.Stores.Add(entity);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(entity.Id))!;
    }

    public async Task<StoreDto?> UpdateAsync(Guid id, UpdateStoreDto store)
    {
        var existing = await _context.Stores.FindAsync(id);
        if (existing == null)
            return null;

        existing.Name = store.Name.Trim();
        existing.Address = string.IsNullOrWhiteSpace(store.Address) ? null : store.Address.Trim();
        existing.City = string.IsNullOrWhiteSpace(store.City) ? null : store.City.Trim();
        existing.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.Stores.FindAsync(id);
        if (existing == null)
            return false;

        var hasServices = await _context.Services.AnyAsync(s => s.StoreId == id);
        if (hasServices)
            throw new InvalidOperationException("No se puede eliminar la tienda porque tiene servicios asociados.");

        _context.Stores.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
