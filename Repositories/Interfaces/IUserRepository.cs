using AutoAlertBackEnd.Dtos;
using AutoAlertBackEnd.Models;

namespace AutoAlertBackEnd.Repositories;

public interface IUserRepository
{
    Task<PagedUsersDto> GetAllUsersAsync(
        int page,
        int pageSize,
        Guid? roleId = null,
        string? search = null,
        bool? isActive = null);
    Task<Users?> GetUserByIdAsync(Guid id);
    Task<Users> CreateUserAsync(CreateUserDto newUser);
    Task<Users?> UpdateUserAsync(Guid id, UpdateUserDto user);
    Task<Users?> UpdateOwnProfileAsync(Guid id, UpdateOwnProfileDto profile);
    Task<bool> UpdateOwnPasswordAsync(Guid id, string password);
    Task<bool> DeleteUserAsync(Guid id);
    Task<Users?> GetUserByEmailAsync(string email);
    Task UpdateLastLoginAsync(Guid userId);
    Task<UserQuantitiesDto> GetUserQuantitiesAsync();
}
