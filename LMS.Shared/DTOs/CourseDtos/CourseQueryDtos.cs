using LMS.Shared.DTOs.PagingDtos;

namespace LMS.Shared.DTOs.CourseDtos;

public class CoursesQueryDto : BasePageQueryDto
{

}

public class CoursesQueryResultDto : BasePagedResultDto<CourseListItemDto>
{

}

public class CourseListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
