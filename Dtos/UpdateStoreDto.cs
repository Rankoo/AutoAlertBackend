using System.ComponentModel.DataAnnotations;

namespace AutoAlertBackEnd.Dtos;

public class UpdateStoreDto
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(150)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }
}
