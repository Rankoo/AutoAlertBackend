using System.ComponentModel.DataAnnotations;

namespace AutoAlertBackEnd.Dtos;

public sealed class UpdateAlertStatusDto
{
    [Required]
    public string Status { get; init; } = string.Empty;
}
