using AutoAlertBackEnd.Context;
using AutoAlertBackEnd.Models;
using AutoAlertBackEnd.NotificationDelivery;
using Microsoft.EntityFrameworkCore;

namespace AutoAlertBackEnd.Repositories;

public class NotificationRepository : INotificationRepository
{
    private static readonly string[] SupportedChannels = ["Email", "WhatsApp", "SMS"];
    private readonly AutoAlertContext _context;
    private readonly INotificationDeliveryQueue _notificationDeliveryQueue;

    public NotificationRepository(AutoAlertContext context, INotificationDeliveryQueue notificationDeliveryQueue)
    {
        _context = context;
        _notificationDeliveryQueue = notificationDeliveryQueue;
    }

    public async Task<IEnumerable<Notifications>> GetAllAsync()
    {
        return await _context.Notifications
            .AsNoTracking()
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<Notifications?> GetByIdAsync(Guid id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    public async Task<IEnumerable<Notifications>> GetByAlertIdAsync(Guid alertId)
    {
        return await _context.Notifications
            .Where(n => n.AlertId == alertId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notifications>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .AsNoTracking()
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<Notifications> CreateAsync(Notifications notification)
    {
        var recipient = await _context.Users.FirstOrDefaultAsync(u => u.Id == notification.UserId && u.IsActive);
        if (recipient is null)
            throw new InvalidOperationException("El usuario seleccionado no existe o está inactivo.");
        if (!await _context.Alerts.AnyAsync(a => a.Id == notification.AlertId))
            throw new InvalidOperationException("La alerta seleccionada no existe.");

        notification.Title = string.IsNullOrWhiteSpace(notification.Title) ? "Nueva notificación" : notification.Title.Trim();
        notification.Message = string.IsNullOrWhiteSpace(notification.Message) ? "Tienes una notificación pendiente." : notification.Message.Trim();
        if (!SupportedChannels.Contains(notification.Channel, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("Selecciona un canal de envío válido.");
        notification.Channel = SupportedChannels.First(channel => channel.Equals(notification.Channel, StringComparison.OrdinalIgnoreCase));
        notification.Result ??= "Pendiente";
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        await _notificationDeliveryQueue.EnqueueAsync(notification.Id);
        return notification;
    }

    public async Task<Notifications?> UpdateAsync(Notifications notification)
    {
        var existing = await _context.Notifications.FindAsync(notification.Id);
        if (existing == null) return null;
        _context.Entry(existing).CurrentValues.SetValues(notification);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> MarkAsReadAsync(Guid id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification is null)
            return false;

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<bool> MarkAsReadAsync(Guid id, Guid userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (notification == null)
            return false;

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<int> MarkAllAsReadAsync(Guid userId)
    {
        var unreadNotifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        if (unreadNotifications.Count == 0)
            return 0;

        var updatedAt = DateTime.Now;
        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.UpdatedAt = updatedAt;
        }

        await _context.SaveChangesAsync();
        return unreadNotifications.Count;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.Notifications.FindAsync(id);
        if (existing == null) return false;
        _context.Notifications.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}

