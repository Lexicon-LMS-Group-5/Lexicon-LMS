using LMS.Shared.DTOs.PagingDtos;
using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs
{
    public class CourseUpsertDto
    {
        public virtual string Name { get; set; } = string.Empty;
        public virtual string Description { get; set; } = string.Empty;
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
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
        public List<CourseParticipantWithRoleInfoDto> Participants { get; set; } = [];
        public List<CourseModuleListItemDto> Modules { get; set; } = [];
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
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public List<ActivityReadDto> Activities { get; set; } = [];
	}

    public class CoursesQueryDto : BasePageQueryDto
    {

    }

    public class CoursesQueryResultDto : BasePagedResultDto<CourseListItemDto>
    {
        
    }

    public class CreateCourseCommandDto : CourseUpsertDto, IValidatableObject
    {
        public string CreatorId { get; set; } = string.Empty;

        [Required]
        [StringLength(35, MinimumLength = 1)]
        public override string Name { get; set; } = string.Empty;

        [StringLength(160)]
        public override string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public override DateTime? StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public override DateTime? EndDate { get; set; }

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

    public class CreateCourseResultDto: CourseReadDto
    {

    }

    public class CourseListItemDto : CourseReadDto
    { 

    }

}
