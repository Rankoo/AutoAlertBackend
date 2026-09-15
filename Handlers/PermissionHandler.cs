using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

public class PermissionHandler 
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;

    public PermissionHandler(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return;

        var hasPermission = await _permissionService
            .HasPermissionAsync(Guid.Parse(userId), requirement.Permission);

        if (hasPermission)
            context.Succeed(requirement);
    }
}