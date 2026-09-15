using System.ComponentModel.DataAnnotations;

namespace AutoAlertBackEnd.Models
{
    public class ExternalAuthDto
    {
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }
}