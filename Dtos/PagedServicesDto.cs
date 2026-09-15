namespace AutoAlertBackEnd.Dtos;

public class PagedServicesDto
{
    public IEnumerable<ServiceDto> Services { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}
