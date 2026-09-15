namespace AutoAlertBackEnd.Dtos;

public sealed class OwnProfileDto
{
    public required string Names { get; init; }
    public string? LastNames { get; init; }
    public required string Email { get; init; }
    public Guid DocumentTypeId { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Address { get; init; }
    public string? DocumentNumber { get; init; }
}
