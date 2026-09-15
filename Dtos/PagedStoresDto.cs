namespace AutoAlertBackEnd.Dtos;

public class PagedStoresDto
{
    public IEnumerable<StoreListDto> Stores { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}
