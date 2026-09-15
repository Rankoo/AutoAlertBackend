using AutoAlertBackEnd.Models;

namespace AutoAlertBackEnd.Repositories;

public interface IAlertRepository
{
    Task<IEnumerable<Alerts>> GetAllAsync();
    Task<IEnumerable<Alerts>> GetByUserIdAsync(Guid userId);
    Task<Alerts?> GetByIdAsync(Guid id);
    Task<IEnumerable<Alerts>> GetByServiceIdAsync(Guid serviceId);
    Task<IEnumerable<Alerts>> GetScheduledAlertsAsync(DateTime? fromDate = null);
    Task<Alerts> CreateAsync(Alerts alert);
    Task<Alerts?> UpdateAsync(Alerts alert);
    Task<Alerts?> UpdateStatusAsync(Guid id, string status);
    Task<bool> DeleteAsync(Guid id);
}

