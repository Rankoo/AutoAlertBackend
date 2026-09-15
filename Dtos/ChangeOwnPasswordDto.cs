using System.ComponentModel.DataAnnotations;

namespace AutoAlertBackEnd.Dtos;

public sealed class ChangeOwnPasswordDto
{
    [Required, MinLength(8), MaxLength(255)]
    public required string NewPassword { get; init; }

    [Required, MinLength(8), MaxLength(255)]
    public required string ConfirmPassword { get; init; }
}
