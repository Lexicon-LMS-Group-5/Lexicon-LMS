using AutoMapper;
using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Hosting;
using Service.Contracts;

namespace LMS.Services;

public class AttachmentService : IAttachmentService
{
    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;
    private readonly IWebHostEnvironment env;

    public AttachmentService(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IWebHostEnvironment env)
    {
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.env = env ?? throw new ArgumentNullException(nameof(env));
    }

    public async Task<AttachmentReadDto> UploadAttachmentAsync(
        AttachmentUpsertDto dto,
        string userId,
        CancellationToken ct)
    {
        var activity = await unitOfWork.Activities.GetByIdAsync(dto.ActivityId, false, ct);
        if (activity == null)
            throw new NotFoundException($"Activity {dto.ActivityId} not found");

        var user = await unitOfWork.Users.GetByIdAsync(userId, false, ct);
        if (user == null)
            throw new NotFoundException($"User {userId} not found");

        if (dto.File == null || dto.File.Length == 0)
            throw new BadRequestException("File is required");

        var extension = Path.GetExtension(dto.File.FileName).ToLowerInvariant();
        var allowedExtensions = new[] { ".pdf", ".zip" };

        if (!allowedExtensions.Contains(extension))
            throw new BadRequestException("Only PDF and ZIP files are allowed");

        var uploadsFolder = Path.Combine(env.ContentRootPath, "Uploads", "Attachments");
        Directory.CreateDirectory(uploadsFolder);

        var storedFileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(uploadsFolder, storedFileName);

        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream, ct);
        }

        var attachment = new Attachment
        {
            ActivityId = dto.ActivityId,
            Activity = activity,
            FileName = dto.File.FileName,
            FilePath = fullPath,
            ContentType = dto.File.ContentType,
            FileSize = dto.File.Length,
            UploadedAt = DateTime.UtcNow,
            UploadedByUserId = userId,
            UploadedByUser = user
        };

        unitOfWork.Attachments.CreateAttachment(attachment);
        await unitOfWork.CompleteAsync(ct);

        return mapper.Map<AttachmentReadDto>(attachment);
    }

    public async Task<List<AttachmentReadDto>> GetAttachmentsByActivityIdAsync(int activityId, CancellationToken ct)
    {
        var activity = await unitOfWork.Activities.GetByIdAsync(activityId, false, ct);
        if (activity == null)
            throw new NotFoundException($"Activity {activityId} not found");

        var attachments = await unitOfWork.Attachments.GetByActivityIdAsync(activityId, false, ct);
        return mapper.Map<List<AttachmentReadDto>>(attachments);
    }

    public async Task DeleteAttachmentAsync(int attachmentId, CancellationToken ct)
    {
        var attachment = await unitOfWork.Attachments.GetByIdAsync(attachmentId, true, ct);
        if (attachment == null)
            throw new NotFoundException($"Attachment {attachmentId} not found");

        if (!string.IsNullOrWhiteSpace(attachment.FilePath) && File.Exists(attachment.FilePath))
            File.Delete(attachment.FilePath);

        unitOfWork.Attachments.DeleteAttachment(attachment);
        await unitOfWork.CompleteAsync(ct);
    }
}