using LMS.Shared.DTOs.PagingDtos;
using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs
{
    public class CourseUpsertDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class CourseReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class CourseDetailsDto : CourseReadDto
    {
        public IReadOnlyList<CourseParticipantDto> Participants { get; set; } = [];
        public IReadOnlyList<ModuleReadDto> Modules { get; set; } = [];
    }

    public class CourseParticipantDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IReadOnlyList<string> Roles { get; set; } = [];
    }

    public class CoursesQueryDto : BasePageQueryDto
    {

    }

    public class CoursesQueryResultDto : BasePagedResultDto<CourseListItemDto>
    {
        
    }

    public class CreateCourseCommandDto : IValidatableObject
    {
        public string CreatorId { get; set; } = string.Empty;

        [Required]
        [StringLength(35, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [StringLength(160)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name="Start date")]
        public DateTime? StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End date")]
        public DateTime? EndDate { get; set; }

        [Required]
        public bool AddCreator { get; set; }

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

    public class EditCourseCommandDto : CreateCourseCommandDto
    {
        public int Id { get; set; }
        public EditCourseCommandDto(CourseDetailsDto courseDetails)
        {
            Id = courseDetails.Id;
            Name = courseDetails.Name;
            Description = courseDetails.Description;
            StartDate = courseDetails.StartDate;
            EndDate = courseDetails.EndDate;
        }
    }

    public class CourseListItemDto : CourseReadDto
    { 
        public int StudentsCount { get; set; }
        public int ModulesCount { get; set; }
    }

}
