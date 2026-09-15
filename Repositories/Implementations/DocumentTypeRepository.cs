using AutoAlertBackEnd.Context;
using AutoAlertBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoAlertBackEnd.Repositories;

public class DocumentTypeRepository : IDocumentTypeRepository
{
    private readonly AutoAlertContext _context;

    public DocumentTypeRepository(AutoAlertContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DocumentTypes>> GetAllAsync()
    {
        return await _context.DocumentTypes.ToListAsync();
    }

    public async Task<DocumentTypes?> GetByIdAsync(Guid id)
    {
        return await _context.DocumentTypes.FindAsync(id);
    }

    public async Task<DocumentTypes> CreateAsync(DocumentTypes documentType)
    {
        _context.DocumentTypes.Add(documentType);
        await _context.SaveChangesAsync();
        return documentType;
    }

    public async Task<DocumentTypes?> UpdateAsync(DocumentTypes documentType)
    {
        var existing = await _context.DocumentTypes.FindAsync(documentType.Id);
        if (existing == null) return null;
        _context.Entry(existing).CurrentValues.SetValues(documentType);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.DocumentTypes.FindAsync(id);
        if (existing == null) return false;
        _context.DocumentTypes.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}

