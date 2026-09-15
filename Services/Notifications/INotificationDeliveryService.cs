using AutoAlertBackEnd.Models;

namespace AutoAlertBackEnd.NotificationDelivery;

public interface INotificationDeliveryService
{
    Task<NotificationDeliveryResult> DeliverAsync(Notifications notification, Users recipient, CancellationToken cancellationToken = default);
}
