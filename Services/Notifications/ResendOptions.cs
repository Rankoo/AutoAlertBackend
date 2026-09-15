namespace AutoAlertBackEnd.NotificationDelivery;

public sealed class ResendOptions
{
    public const string SectionName = "Resend";

    public string ApiKey { get; init; } = string.Empty;
    public string SenderEmail { get; init; } = string.Empty;
    public string SenderName { get; init; } = "AutoAlert";
    public string TemplateId { get; init; } = "alert-template";

    public bool IsConfigured => !string.IsNullOrWhiteSpace(ApiKey)
        && !string.IsNullOrWhiteSpace(SenderEmail)
        && !string.IsNullOrWhiteSpace(TemplateId);
}
