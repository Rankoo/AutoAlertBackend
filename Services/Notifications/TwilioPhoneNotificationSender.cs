using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Text.Json;
using AutoAlertBackEnd.Models;
using Microsoft.Extensions.Options;

namespace AutoAlertBackEnd.NotificationDelivery;

public sealed class TwilioPhoneNotificationSender : IPhoneNotificationSender
{
    private static readonly Regex E164PhoneNumber = new("^\\+[1-9]\\d{7,14}$", RegexOptions.Compiled);
    private readonly HttpClient _httpClient;
    private readonly TwilioOptions _options;
    private readonly ILogger<TwilioPhoneNotificationSender> _logger;

    public TwilioPhoneNotificationSender(
        HttpClient httpClient,
        IOptions<TwilioOptions> options,
        ILogger<TwilioPhoneNotificationSender> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public Task<NotificationDeliveryResult> SendSmsAsync(
        Notifications notification,
        Users recipient,
        CancellationToken cancellationToken = default)
        => SendAsync(notification, recipient, _options.IsSmsConfigured, _options.SmsFrom, false, cancellationToken);

    public Task<NotificationDeliveryResult> SendWhatsAppAsync(
        Notifications notification,
        Users recipient,
        CancellationToken cancellationToken = default)
        => SendAsync(notification, recipient, _options.IsWhatsAppConfigured, _options.WhatsAppFrom, true, cancellationToken);

    private async Task<NotificationDeliveryResult> SendAsync(
        Notifications notification,
        Users recipient,
        bool isConfigured,
        string sender,
        bool isWhatsApp,
        CancellationToken cancellationToken)
    {
        var channel = isWhatsApp ? "WhatsApp" : "SMS";
        if (!_options.Enabled)
            return new(false, $"Desactivado: canal {channel}");

        if (!isConfigured)
            return new(false, $"Pendiente: configura Twilio para {channel}");

        if (!TryFormatPhoneNumber(recipient.PhoneNumber, isWhatsApp, out var recipientPhoneNumber))
            return new(false, "Fallido: usuario sin teléfono en formato E.164");

        if (!TryFormatPhoneNumber(sender, isWhatsApp, out var senderPhoneNumber))
            return new(false, $"Fallido: remitente de {channel} inválido");

        var message = string.IsNullOrWhiteSpace(notification.Title)
            ? notification.Message ?? "Tienes una notificación pendiente."
            : $"{notification.Title}\n{notification.Message}".Trim();
        var form = new Dictionary<string, string>
        {
            ["From"] = senderPhoneNumber,
            ["To"] = recipientPhoneNumber,
            ["Body"] = message,
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.AccountSid}/Messages.json")
        {
            Content = new FormUrlEncodedContent(form),
        };
        var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{_options.AccountSid}:{_options.AuthToken}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        try
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
                return new(true, "Enviado", DateTime.UtcNow);

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var code = TryGetTwilioErrorCode(responseBody);
            _logger.LogWarning(
                "Twilio rechazó la notificación {NotificationId} por {Channel} con estado {StatusCode} y código {TwilioCode}",
                notification.Id,
                channel,
                (int)response.StatusCode,
                code);
            return new(false, $"Fallido: Twilio {(int)response.StatusCode}{(code is null ? string.Empty : $" ({code})")}");
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "No se pudo enviar la notificación {NotificationId} por {Channel} mediante Twilio", notification.Id, channel);
            return new(false, "Fallido: proveedor no disponible");
        }
    }

    private static bool TryFormatPhoneNumber(string? phoneNumber, bool isWhatsApp, out string formattedPhoneNumber)
    {
        var number = (phoneNumber ?? string.Empty).Trim();
        if (number.StartsWith("whatsapp:", StringComparison.OrdinalIgnoreCase))
            number = number["whatsapp:".Length..];

        if (!E164PhoneNumber.IsMatch(number))
        {
            formattedPhoneNumber = string.Empty;
            return false;
        }

        formattedPhoneNumber = isWhatsApp ? $"whatsapp:{number}" : number;
        return true;
    }

    private static string? TryGetTwilioErrorCode(string responseBody)
    {
        try
        {
            using var document = JsonDocument.Parse(responseBody);
            return document.RootElement.TryGetProperty("code", out var code) ? code.ToString() : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
