using System.ComponentModel.DataAnnotations;
namespace AutoAlertBackEnd.Dtos;
public class CreateServiceDto
{
    [Required] public Guid StoreId { get; set; }
    [Required, MaxLength(150)] public required string Name { get; set; }
    [MaxLength(100)] public string? Provider { get; set; }
    [MaxLength(100)] public string? AccountNumber { get; set; }
}
