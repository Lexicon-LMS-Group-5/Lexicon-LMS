namespace LMS.Shared.DTOs
{
    public class CourseUpsertDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class CourseReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public record CourseDetailsQueryDto(int CourseId);

    public class CourseDetailsDto : CourseReadDto
    {
        public IEnumerable<CourseParticipantWithRoleInfoDto> Participants { get; set; } = [];
        public IEnumerable<CourseModuleListItemDto> Modules { get; set; } = [];
    }

    public class CourseParticipantWithRoleInfoDto
    {
        public string Id { get; set; } = default!;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class CourseModuleListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
