namespace AutoAlertBackEnd.Dtos;

public class ServiceDto
{
    public Guid Id { get; set; }
    public Guid StoreId { get; set; }
    public string? StoreName { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Provider { get; set; }
    public string? AccountNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
