namespace AutoAlertBackEnd.NotificationDelivery;

public sealed record NotificationDeliveryResult(bool IsSuccessful, string Result, DateTime? SentAt = null);
