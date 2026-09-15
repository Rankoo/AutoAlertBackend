using System.ComponentModel.DataAnnotations;

namespace AutoAlertDB.Models
{
    public class ExternalAuthDto
    {
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }
}