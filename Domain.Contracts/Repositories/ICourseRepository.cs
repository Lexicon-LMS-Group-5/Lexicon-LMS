using Domain.Models.Entities;

namespace Domain.Contracts.Repositories
{
    public interface ICourseRepository : IInternalRepositoryBase<Course>, IRepositoryBase<Course>
    {
        Course? GetCourseDetailsById(int courseId, bool trackChanges = false);
        Task<Course?> GetCourseDetailsByIdAsync(int courseId, bool trackChanges = false);
    }
}
