
namespace Domain.Models.Entities
{
    public class Attachment
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public required Activity Activity { get; set; }

        public string FileName { get; set; } = "";
        public string FilePath { get; set; } = "";
        public string? ContentType { get; set; }
        public long? FileSize { get; set; }

        public DateTime UploadedAt { get; set; }

        public string UploadedByUserId { get; set; } = "";
        public required ApplicationUser UploadedByUser { get; set; }
    }
}
