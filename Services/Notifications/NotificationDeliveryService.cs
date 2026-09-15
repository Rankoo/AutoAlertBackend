using AutoAlertBackEnd.Models;

namespace AutoAlertBackEnd.NotificationDelivery;

public sealed class NotificationDeliveryService : INotificationDeliveryService
{
    private readonly IEmailNotificationSender _emailSender;
    private readonly IPhoneNotificationSender _phoneSender;

    public NotificationDeliveryService(IEmailNotificationSender emailSender, IPhoneNotificationSender phoneSender)
    {
        _emailSender = emailSender;
        _phoneSender = phoneSender;
    }

    public Task<NotificationDeliveryResult> DeliverAsync(Notifications notification, Users recipient, CancellationToken cancellationToken = default)
    {
        return notification.Channel switch
        {
            "Email" => _emailSender.SendAsync(notification, recipient, cancellationToken),
            "WhatsApp" => _phoneSender.SendWhatsAppAsync(notification, recipient, cancellationToken),
            "SMS" => _phoneSender.SendSmsAsync(notification, recipient, cancellationToken),
            _ => Task.FromResult(new NotificationDeliveryResult(false, "Fallido: canal no válido")),
        };
    }
}
