using System.ComponentModel.DataAnnotations;

namespace AutoAlertBackEnd.Dtos
{
    public class CreateUserDto
    {
        [Required]
        [MaxLength(150)]
        public required string Names { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public required string LastNames { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public required string Email { get; set; } = string.Empty;

        [Phone]
        [MaxLength(20)]
        public required string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(255)]
        public required string Address { get; set; } = string.Empty;

        [MaxLength(50)]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        public Guid DocumentTypeId { get; set; }

        public Guid? CompanyId { get; set; }

        [MaxLength(100)]
        public required string Position { get; set; } = string.Empty;

        [Required]
        public required Guid RoleId { get; set; }

        [MaxLength(255)]
        public string TemporalPassword { get; set; } = string.Empty;

        public bool ChangePassword { get; set; } = true;
        public bool IsActive { get; set; } = true;

    }
}
