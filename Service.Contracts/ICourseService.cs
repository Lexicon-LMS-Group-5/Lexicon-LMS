using LMS.Shared.DTOs.CourseDtos;

namespace Service.Contracts
{
    public interface ICourseService
    {
        Task<CourseDetailsDto> GetCourseDetailsAsync(CourseDetailsQueryDto query, CancellationToken ct = default);
    }
}
