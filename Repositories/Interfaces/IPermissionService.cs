public interface IPermissionService
{
    Task<bool> HasPermissionAsync(Guid userId, string permission);
}