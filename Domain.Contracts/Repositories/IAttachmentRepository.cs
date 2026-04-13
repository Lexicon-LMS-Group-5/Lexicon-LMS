using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface IAttachmentRepository
{
    Task<Attachment?> GetByIdAsync(int id, bool trackChanges, CancellationToken ct);
    Task<List<Attachment>> GetByActivityIdAsync(int activityId, bool trackChanges, CancellationToken ct);
    void CreateAttachment(Attachment attachment);
    void DeleteAttachment(Attachment attachment);
}