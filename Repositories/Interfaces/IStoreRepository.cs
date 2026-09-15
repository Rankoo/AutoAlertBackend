using AutoAlertBackEnd.Dtos;

namespace AutoAlertBackEnd.Repositories;

public interface IStoreRepository
{
    Task<PagedStoresDto> GetAllAsync(int page, int pageSize, string? search = null);
    Task<StoreDto?> GetByIdAsync(Guid id);
    Task<StoreQuantitiesDto> GetQuantitiesAsync();
    Task<StoreDto> CreateAsync(CreateStoreDto store);
    Task<StoreDto?> UpdateAsync(Guid id, UpdateStoreDto store);
    Task<bool> DeleteAsync(Guid id);
}
