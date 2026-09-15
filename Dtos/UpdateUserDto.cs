using System.ComponentModel.DataAnnotations;

namespace AutoAlertBackEnd.Dtos;

public class UpdateUserDto
{
    [Required, EmailAddress, MaxLength(150)]
    public required string Email { get; set; }

    [Required, MaxLength(150)]
    public required string Names { get; set; }

    [MaxLength(150)]
    public string? LastNames { get; set; }

    [MaxLength(255)]
    public string? Address { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(50)]
    public string? DocumentNumber { get; set; }

    public Guid DocumentTypeId { get; set; }
    public Guid RoleId { get; set; }

    [MaxLength(100)]
    public string? Position { get; set; }

    [MaxLength(255)]
    public string? Password { get; set; }

    public bool ChangePassword { get; set; }
    public bool IsActive { get; set; }
}