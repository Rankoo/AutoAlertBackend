namespace AutoAlertBackEnd.Dtos;

public class UserQuantitiesDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int Administrators { get; set; }
    public int UsersCreatedLastSevenDays { get; set; }
}