namespace LMS.Shared.DTOs
{
    public class ModuleUpsertDto
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class ModuleReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ActivityReadDto> Activities { get; set; } = [];
        public int CourseId { get; set; }
        public string CourseName { get; set; } = "";
    }
}
