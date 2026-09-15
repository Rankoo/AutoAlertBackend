using AutoAlertBackEnd.Models;

namespace AutoAlertBackEnd.Repositories;

public interface IUserGroupRepository
{
    Task<IEnumerable<UserGroups>> GetAllAsync();
    Task<UserGroups?> GetByIdAsync(Guid userId, Guid groupId);
    Task<IEnumerable<UserGroups>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<UserGroups>> GetByGroupIdAsync(Guid groupId);
    Task<UserGroups> CreateAsync(UserGroups userGroup);
    Task<UserGroups?> UpdateAsync(UserGroups userGroup);
    Task<bool> DeleteAsync(Guid userId, Guid groupId);
}

