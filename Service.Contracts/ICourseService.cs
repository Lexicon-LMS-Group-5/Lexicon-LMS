using LMS.Shared.DTOs;

namespace Service.Contracts
{
    public interface ICourseService
    {
        Task<CoursesQueryResultDto> GetCoursesAsync(CoursesQueryDto query, CancellationToken ct = default);
        Task<CourseDetailsDto> GetCourseDetailsAsync(CourseDetailsQueryDto query, CancellationToken ct = default);
        Task<CreateCourseResultDto> CreateCourseAsync(CreateCourseCommandDto command, CancellationToken ct = default);
    }
}
