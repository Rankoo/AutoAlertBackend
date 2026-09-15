using System.Text;
using System.Text.Json;
using AutoAlertBackEnd.Models;
using Microsoft.Extensions.Options;

namespace AutoAlertBackEnd.NotificationDelivery;

public sealed class ResendEmailNotificationSender : IEmailNotificationSender
{
    private readonly HttpClient _httpClient;
    private readonly ResendOptions _options;
    private readonly ILogger<ResendEmailNotificationSender> _logger;

    public ResendEmailNotificationSender(HttpClient httpClient, IOptions<ResendOptions> options, ILogger<ResendEmailNotificationSender> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<NotificationDeliveryResult> SendAsync(Notifications notification, Users recipient, CancellationToken cancellationToken = default)
    {
        if (!_options.IsConfigured)
            return new(false, "Pendiente: configura Resend");

        if (string.IsNullOrWhiteSpace(recipient.Email))
            return new(false, "Fallido: usuario sin email");

        var sender = string.IsNullOrWhiteSpace(_options.SenderName)
            ? _options.SenderEmail
            : $"{_options.SenderName} <{_options.SenderEmail}>";
        var recipientName = string.Join(" ", new[] { recipient.Names, recipient.LastNames }
            .Where(name => !string.IsNullOrWhiteSpace(name)));
        var payload = new
        {
            from = sender,
            to = new[] { recipient.Email },
            subject = notification.Title ?? "Nueva notificación",
            template = new
            {
                id = _options.TemplateId,
                variables = new
                {
                    anio = DateTime.UtcNow.Year.ToString(),
                    asunto = notification.Title ?? "Nueva notificación",
                    mensaje = notification.Message ?? "Tienes una notificación pendiente.",
                    nombre = string.IsNullOrWhiteSpace(recipientName) ? "Usuario" : recipientName
                }
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "emails")
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"),
        };
        request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_options.ApiKey}");
        request.Headers.TryAddWithoutValidation("Idempotency-Key", $"notification-{notification.Id}");

        try
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
                return new(true, "Enviado", DateTime.UtcNow);

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning(
                "Resend rechazó la notificación {NotificationId} con estado {StatusCode}: {ResponseBody}",
                notification.Id,
                (int)response.StatusCode,
                responseBody);
            return new(false, $"Fallido: Resend {(int)response.StatusCode}");
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "No se pudo enviar la notificación {NotificationId} mediante Resend", notification.Id);
            return new(false, "Fallido: proveedor no disponible");
        }
    }
}
