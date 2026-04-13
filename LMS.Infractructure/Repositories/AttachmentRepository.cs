using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class AttachmentRepository : RepositoryBase<Attachment>, IAttachmentRepository
{
    public AttachmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Attachment?> GetByIdAsync(int id, bool trackChanges, CancellationToken ct) =>
        await FindByCondition(a => a.Id == id, trackChanges)
            .SingleOrDefaultAsync(ct);

    public async Task<List<Attachment>> GetByActivityIdAsync(int activityId, bool trackChanges, CancellationToken ct) =>
        await FindByCondition(a => a.ActivityId == activityId, trackChanges)
            .OrderByDescending(a => a.UploadedAt)
            .ToListAsync(ct);

    public void CreateAttachment(Attachment attachment) => Create(attachment);

    public void DeleteAttachment(Attachment attachment) => Delete(attachment);
}