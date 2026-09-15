namespace AutoAlertBackEnd.Dtos;

public class StoreQuantitiesDto
{
    public int TotalStores { get; set; }
    public int StoresWithServices { get; set; }
    public int CitiesCount { get; set; }
    public int StoresCreatedLastSevenDays { get; set; }
}
