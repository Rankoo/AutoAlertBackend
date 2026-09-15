namespace AutoAlertBackEnd.Dtos
{
    public class UserRolePermissionsDto
    {
        required public string Role { get; set; }
        public List<string>? SpecialPermissions { get; set; }
    }
}