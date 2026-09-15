using AutoAlertBackEnd.Models;

namespace AutoAlertBackEnd.Repositories;

public interface ILogRepository
{
    Task<IEnumerable<Logs>> GetAllAsync();
    Task<Logs?> GetByIdAsync(Guid id);
    Task<IEnumerable<Logs>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Logs>> GetByTableNameAsync(string tableName);
    Task<IEnumerable<Logs>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Logs> CreateAsync(Logs log);
    Task<bool> DeleteAsync(Guid id);
}

