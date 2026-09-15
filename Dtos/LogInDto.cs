using System.ComponentModel.DataAnnotations;

namespace AutoAlertBackEnd.Dtos
{
    public class LogInDto    {
        [Required]
        public required string Email { get; set; } = string.Empty!;

        [Required]
        public required string Password { get; set; } = string.Empty!;
    }
}
