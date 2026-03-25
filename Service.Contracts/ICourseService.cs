using LMS.Shared.DTOs;

namespace Service.Contracts
{
    public interface ICourseService
    {
        Task<CourseDetailsDto> GetCourseDetailsAsync(CourseDetailsQueryDto query);
    }
}
