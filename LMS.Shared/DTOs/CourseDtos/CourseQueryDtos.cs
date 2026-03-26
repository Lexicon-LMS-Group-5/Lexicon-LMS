using LMS.Shared.DTOs.PagingDtos;

namespace LMS.Shared.DTOs.CourseDtos;

public class CoursesQueryDto : BasePageQueryDto
{

}

public class CoursesQueryResultDto : BasePagedResultDto<CourseListItemDto>
{

}

public class CourseListItemDto : CourseReadDto
{

}
