using AutoAlertBackEnd.Models;

namespace AutoAlertBackEnd.Repositories;

public interface IDocumentTypeRepository
{
    Task<IEnumerable<DocumentTypes>> GetAllAsync();
    Task<DocumentTypes?> GetByIdAsync(Guid id);
    Task<DocumentTypes> CreateAsync(DocumentTypes documentType);
    Task<DocumentTypes?> UpdateAsync(DocumentTypes documentType);
    Task<bool> DeleteAsync(Guid id);
}
