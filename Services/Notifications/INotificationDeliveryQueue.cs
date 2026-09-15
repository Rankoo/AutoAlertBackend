namespace AutoAlertBackEnd.NotificationDelivery;

public interface INotificationDeliveryQueue
{
    ValueTask EnqueueAsync(Guid notificationId, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Guid> ReadAllAsync(CancellationToken cancellationToken);
}
