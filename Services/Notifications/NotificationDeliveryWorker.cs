using AutoAlertBackEnd.Context;
using Microsoft.EntityFrameworkCore;

namespace AutoAlertBackEnd.NotificationDelivery;

public sealed class NotificationDeliveryWorker : BackgroundService
{
    private readonly INotificationDeliveryQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationDeliveryWorker> _logger;

    public NotificationDeliveryWorker(
        INotificationDeliveryQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<NotificationDeliveryWorker> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await QueuePendingNotificationsAsync(stoppingToken);

        await foreach (var notificationId in _queue.ReadAllAsync(stoppingToken))
        {
            try
            {
                await DeliverAsync(notificationId, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "No se pudo procesar la notificación {NotificationId}", notificationId);
            }
        }
    }

    private async Task QueuePendingNotificationsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AutoAlertContext>();
        var pendingIds = await context.Notifications
            .Where(notification => notification.SentAt == null && notification.Result == "Pendiente")
            .Select(notification => notification.Id)
            .ToListAsync(cancellationToken);

        foreach (var notificationId in pendingIds)
            await _queue.EnqueueAsync(notificationId, cancellationToken);
    }

    private async Task DeliverAsync(Guid notificationId, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AutoAlertContext>();
        var deliveryService = scope.ServiceProvider.GetRequiredService<INotificationDeliveryService>();
        var notification = await context.Notifications
            .Include(item => item.User)
            .SingleOrDefaultAsync(item => item.Id == notificationId, cancellationToken);

        if (notification is null)
            return;

        if (notification.User is null || !notification.User.IsActive)
        {
            notification.Result = "Fallido: usuario inexistente o inactivo";
            notification.UpdatedAt = DateTime.Now;
            await context.SaveChangesAsync(cancellationToken);
            return;
        }

        if (notification.SentAt is not null || notification.Result != "Pendiente")
            return;

        var delivery = await deliveryService.DeliverAsync(notification, notification.User, cancellationToken);
        notification.Result = delivery.Result;
        notification.SentAt = delivery.SentAt;
        notification.UpdatedAt = DateTime.Now;
        await context.SaveChangesAsync(cancellationToken);
    }
}
