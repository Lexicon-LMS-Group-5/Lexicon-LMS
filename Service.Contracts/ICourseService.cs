using LMS.Shared.DTOs.CourseDtos;

namespace Service.Contracts
{
    public interface ICourseService
    {
        Task<CoursesQueryResultDto> GetCoursesAsync(CoursesQueryDto query, CancellationToken ct = default);
        Task<CourseDetailsDto> GetCourseDetailsAsync(CourseDetailsQueryDto query, CancellationToken ct = default);
    }
}
