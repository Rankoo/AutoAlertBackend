using AutoAlertBackEnd.Context;
using AutoAlertBackEnd.Dtos;
using AutoAlertBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoAlertBackEnd.Repositories;

public class ServiceRepository(AutoAlertContext context) : IServiceRepository
{
    public async Task<PagedServicesDto> GetAllAsync(int page, int pageSize, string? search = null, Guid? storeId = null)
    {
        var query = context.Services.AsNoTracking();
        if (storeId.HasValue) query = query.Where(s => s.StoreId == storeId.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = $"%{search.Trim()}%";
            query = query.Where(s => EF.Functions.Like(s.Name, term) ||
                (s.Provider != null && EF.Functions.Like(s.Provider, term)) ||
                (s.AccountNumber != null && EF.Functions.Like(s.AccountNumber, term)));
        }

        var projection = query.OrderBy(s => s.Name).Select(s => new ServiceDto
        {
            Id = s.Id, StoreId = s.StoreId, StoreName = s.Store != null ? s.Store.Name : null,
            Name = s.Name, Provider = s.Provider, AccountNumber = s.AccountNumber, CreatedAt = s.CreatedAt, UpdatedAt = s.UpdatedAt
        });
        var totalItems = await projection.CountAsync();
        return new PagedServicesDto
        {
            Services = await projection.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(),
            Page = page, PageSize = pageSize, TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public Task<ServiceDto?> GetByIdAsync(Guid id) => context.Services.AsNoTracking().Where(s => s.Id == id)
        .Select(s => new ServiceDto
        {
            Id = s.Id, StoreId = s.StoreId, StoreName = s.Store != null ? s.Store.Name : null,
            Name = s.Name, Provider = s.Provider, AccountNumber = s.AccountNumber, CreatedAt = s.CreatedAt, UpdatedAt = s.UpdatedAt
        }).FirstOrDefaultAsync();

    public async Task<ServiceCatalogsDto> GetCatalogsAsync() => new()
    {
        Stores = await context.Stores.AsNoTracking().OrderBy(s => s.Name).Select(s => new StoreListDto
        {
            Id = s.Id, Name = s.Name, Address = s.Address, City = s.City
        }).ToListAsync()
    };

    public async Task<ServiceQuantitiesDto> GetQuantitiesAsync()
    {
        var today = DateTime.Today;
        return new ServiceQuantitiesDto
        {
            TotalServices = await context.Services.CountAsync(),
            PendingServices = await context.Alerts.CountAsync(a => a.Status == "Pendiente"),
            DueSoon = await context.Alerts.CountAsync(a => a.DueDate >= today && a.DueDate <= today.AddDays(7) && a.Status != "Pagado"),
            PendingAmount = await context.Alerts.Where(a => a.Status != "Pagado").SumAsync(a => a.Amount)
        };
    }

    public async Task<ServiceDto> CreateAsync(CreateServiceDto service)
    {
        await EnsureStoreExists(service.StoreId);
        var entity = new Services { StoreId = service.StoreId, Name = service.Name.Trim(), Provider = Clean(service.Provider), AccountNumber = Clean(service.AccountNumber) };
        context.Services.Add(entity);
        await context.SaveChangesAsync();
        return (await GetByIdAsync(entity.Id))!;
    }

    public async Task<ServiceDto?> UpdateAsync(Guid id, UpdateServiceDto service)
    {
        var entity = await context.Services.FindAsync(id);
        if (entity is null) return null;
        await EnsureStoreExists(service.StoreId);
        entity.StoreId = service.StoreId; entity.Name = service.Name.Trim(); entity.Provider = Clean(service.Provider); entity.AccountNumber = Clean(service.AccountNumber);
        entity.UpdatedAt = DateTime.Now;
        await context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await context.Services.FindAsync(id);
        if (entity is null) return false;
        if (await context.Alerts.AnyAsync(a => a.ServiceId == id)) throw new InvalidOperationException("No se puede eliminar el servicio porque tiene alertas asociadas.");
        context.Services.Remove(entity); await context.SaveChangesAsync(); return true;
    }

    private async Task EnsureStoreExists(Guid storeId)
    {
        if (!await context.Stores.AnyAsync(s => s.Id == storeId)) throw new InvalidOperationException("La tienda seleccionada no existe.");
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
