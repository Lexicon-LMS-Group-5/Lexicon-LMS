using LMS.Shared.DTOs;

namespace Service.Contracts
{
    public interface ICourseService
    {
        Task<CoursesQueryResultDto> GetCoursesAsync(CoursesQueryDto query, CancellationToken ct = default);
        Task<CourseDetailsDto> GetCourseDetailsByIdAsync(int courseId, CancellationToken ct = default);
        Task<CourseDetailsDto> GetCourseDetailsByUserIdAsync(string userId, CancellationToken ct = default);
        Task<CourseReadDto> CreateCourseAsync(CreateCourseCommandDto command, CancellationToken ct = default);
        Task<CourseReadDto> UpdateCourseAsync(int id, CourseUpsertDto dto, CancellationToken ct = default);
        public Task DeleteCourseAsync(int id, CancellationToken ct = default);
    }
}
