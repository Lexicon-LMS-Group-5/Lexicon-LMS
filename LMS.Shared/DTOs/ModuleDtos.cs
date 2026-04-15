using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs
{
    public class ModuleUpsertDto : IValidatableObject
    {
        [Required]
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateRangeRequestDto? TimeCond { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        public DateTime? EndDate { get; set; }
        [Required]
        public int CourseId { get; set; }  // required for Module creation.

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate <= StartDate)
            {
                yield return new ValidationResult("End date must be after the start date.", [nameof(EndDate), nameof(StartDate)]);
            }

            if (EndDate <= DateTime.UtcNow)
            {
                yield return new ValidationResult("End date must be in the future", [nameof(EndDate)]);
            }
        }
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
public class ModuleCourseIdDto
// This class serves to prevent a service-level API from including
// 2 int arguments (for moduleId and courseId) in a way that could be
// confusing for the caller.
{
    public int CourseId { get; set; }
    public ModuleCourseIdDto(int cid) { CourseId = cid; }
}
