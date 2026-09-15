using AutoAlertBackEnd.Models;

namespace AutoAlertBackEnd.NotificationDelivery;

public interface IEmailNotificationSender
{
    Task<NotificationDeliveryResult> SendAsync(Notifications notification, Users recipient, CancellationToken cancellationToken = default);
}
