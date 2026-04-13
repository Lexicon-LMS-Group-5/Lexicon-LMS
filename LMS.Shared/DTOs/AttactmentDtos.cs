using Microsoft.AspNetCore.Http;

namespace LMS.Shared.DTOs
{
    public class AttachmentUpsertDto
    {
        public int ActivityId { get; set; }
        public IFormFile File { get; set; } = default!;
    }

    public class AttachmentReadDto
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public string UploadedByUserId { get; set; } = string.Empty;
    }
}
