using AutoAlertBackEnd.Models;

namespace AutoAlertBackEnd.NotificationDelivery;

public interface IPhoneNotificationSender
{
    Task<NotificationDeliveryResult> SendSmsAsync(Notifications notification, Users recipient, CancellationToken cancellationToken = default);
    Task<NotificationDeliveryResult> SendWhatsAppAsync(Notifications notification, Users recipient, CancellationToken cancellationToken = default);
}
