using AutoAlertBackEnd.Context;
using AutoAlertBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoAlertBackEnd.Repositories;

public class LogRepository : ILogRepository
{
    private readonly AutoAlertContext _context;

    public LogRepository(AutoAlertContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Logs>> GetAllAsync()
    {
        return await _context.Logs.OrderByDescending(l => l.Timestamp).ToListAsync();
    }

    public async Task<Logs?> GetByIdAsync(Guid id)
    {
        return await _context.Logs.FindAsync(id);
    }

    public async Task<IEnumerable<Logs>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Logs
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<Logs>> GetByTableNameAsync(string tableName)
    {
        return await _context.Logs
            .Where(l => l.TableName == tableName)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<Logs>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Logs
            .Where(l => l.Timestamp >= startDate && l.Timestamp <= endDate)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();
    }

    public async Task<Logs> CreateAsync(Logs log)
    {
        _context.Logs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.Logs.FindAsync(id);
        if (existing == null) return false;
        _context.Logs.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}

