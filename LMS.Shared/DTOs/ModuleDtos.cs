namespace LMS.Shared.DTOs
{
    public class ModuleUpsertDto
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateRangeRequestDto? TimeCond;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CourseId { get; set; }  // required for Module creation.
    }

    public class ModuleReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IReadOnlyList<ActivityReadDto> Activities { get; set; } = [];
        public int CourseId { get; set; }
        public string CourseName { get; set; } = "";
    }
}

// This class serves to prevent a service-level API from including
// 2 int arguments (for moduleId and courseId) in a way that could be
// confusing for the caller.
public class ModuleCourseIdDto(int moduleId, int courseId)
{
    public int CourseId { get; set; } = courseId;
    public int ModuleId { get; set; } = moduleId;
}
