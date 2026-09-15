using System.ComponentModel.DataAnnotations;

namespace AutoAlertBackEnd.Dtos;

public sealed class UpdateOwnProfileDto
{
    [Required, MaxLength(150)]
    public required string Names { get; init; }

    [MaxLength(150)]
    public string? LastNames { get; init; }

    public Guid DocumentTypeId { get; init; }

    [Phone, MaxLength(20)]
    public string? PhoneNumber { get; init; }

    [MaxLength(255)]
    public string? Address { get; init; }

    [MaxLength(50)]
    public string? DocumentNumber { get; init; }
}
