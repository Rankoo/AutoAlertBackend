namespace AutoAlertBackEnd.Dtos;

public class UserCatalogsDto
{
    public IEnumerable<CatalogItemDto> Roles { get; set; } = [];
    public IEnumerable<PermissionCatalogDto> Permissions { get; set; } = [];
    public IEnumerable<DocumentTypeCatalogDto> DocumentTypes { get; set; } = [];
}

public class CatalogItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class PermissionCatalogDto
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public Guid SubModuleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string PermissionName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}

public class DocumentTypeCatalogDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Abbreviation { get; set; }
}