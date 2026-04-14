using LMS.Shared.DTOs;

namespace Service.Contracts
{
    public interface ICourseService
    {
        Task<CoursesQueryResultDto> GetCoursesAsync(CoursesQueryDto query, CancellationToken ct = default);
        Task<CourseDetailsDto> GetCourseDetailsAsync(int courseId, CancellationToken ct = default);
        Task<CourseDetailsDto> GetCourseDetailsByUserIdAsync(string userId, CancellationToken ct = default);
        Task<CreateCourseResultDto> CreateCourseAsync(CreateCourseDto command, CancellationToken ct = default);
        Task<CourseDetailsDto> UpdateCourseAsync(CourseUpdateDto command, CancellationToken ct = default);
        public Task DeleteCourseAsync(int id, CancellationToken ct = default);
    }
}
