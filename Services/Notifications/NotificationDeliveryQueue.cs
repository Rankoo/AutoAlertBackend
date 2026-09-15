using System.Threading.Channels;

namespace AutoAlertBackEnd.NotificationDelivery;

public sealed class NotificationDeliveryQueue : INotificationDeliveryQueue
{
    private readonly Channel<Guid> _queue = Channel.CreateUnbounded<Guid>(new UnboundedChannelOptions
    {
        SingleReader = true,
        AllowSynchronousContinuations = false
    });

    public ValueTask EnqueueAsync(Guid notificationId, CancellationToken cancellationToken = default)
        => _queue.Writer.WriteAsync(notificationId, cancellationToken);

    public IAsyncEnumerable<Guid> ReadAllAsync(CancellationToken cancellationToken)
        => _queue.Reader.ReadAllAsync(cancellationToken);
}
