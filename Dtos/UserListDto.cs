namespace AutoAlertBackEnd.Dtos;

public class UserListDto
{
    public Guid Id { get; set; }
    public string? RoleName { get; set; }
    public string Names { get; set; } = string.Empty;
    public string? LastNames { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
}