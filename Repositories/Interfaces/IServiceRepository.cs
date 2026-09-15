using AutoAlertBackEnd.Dtos;

namespace AutoAlertBackEnd.Repositories;

public interface IServiceRepository
{
    Task<PagedServicesDto> GetAllAsync(int page, int pageSize, string? search = null, Guid? storeId = null);
    Task<ServiceDto?> GetByIdAsync(Guid id);
    Task<ServiceCatalogsDto> GetCatalogsAsync();
    Task<ServiceQuantitiesDto> GetQuantitiesAsync();
    Task<ServiceDto> CreateAsync(CreateServiceDto service);
    Task<ServiceDto?> UpdateAsync(Guid id, UpdateServiceDto service);
    Task<bool> DeleteAsync(Guid id);
}

