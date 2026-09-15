namespace AutoAlertBackEnd.NotificationDelivery;

public sealed class TwilioOptions
{
    public const string SectionName = "Twilio";

    public string AccountSid { get; init; } = string.Empty;
    public string AuthToken { get; init; } = string.Empty;
    public string SmsFrom { get; init; } = string.Empty;
    public string WhatsAppFrom { get; init; } = string.Empty;
    public bool Enabled { get; init; }

    public bool HasCredentials => !string.IsNullOrWhiteSpace(AccountSid)
        && !string.IsNullOrWhiteSpace(AuthToken);

    public bool IsSmsConfigured => Enabled && HasCredentials && !string.IsNullOrWhiteSpace(SmsFrom);
    public bool IsWhatsAppConfigured => Enabled && HasCredentials && !string.IsNullOrWhiteSpace(WhatsAppFrom);
}
