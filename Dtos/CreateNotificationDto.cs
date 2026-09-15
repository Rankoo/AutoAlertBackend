using System.ComponentModel.DataAnnotations;

namespace AutoAlertBackEnd.Dtos;

public class CreateNotificationDto
{
    [Required]
    public Guid AlertId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required, MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Channel { get; set; } = string.Empty;
}
