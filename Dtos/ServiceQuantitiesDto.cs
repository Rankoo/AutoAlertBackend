namespace AutoAlertBackEnd.Dtos;

public class ServiceQuantitiesDto
{
    public int TotalServices { get; set; }
    public int PendingServices { get; set; }
    public int DueSoon { get; set; }
    public decimal PendingAmount { get; set; }
}
