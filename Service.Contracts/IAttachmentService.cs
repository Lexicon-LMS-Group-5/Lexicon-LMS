using Domain.Models.Entities;
using LMS.Shared.DTOs;

namespace Service.Contracts;

public interface IAttachmentService
{
    Task<AttachmentReadDto> UploadAttachmentAsync(AttachmentUpsertDto dto, string userId, CancellationToken ct);
    Task<List<AttachmentReadDto>> GetAttachmentsByActivityIdAsync(int activityId, CancellationToken ct);
    Task<Attachment> GetAttachmentByIdAsync(int attachmentId, CancellationToken ct);
    Task DeleteAttachmentAsync(int attachmentId, CancellationToken ct);
}