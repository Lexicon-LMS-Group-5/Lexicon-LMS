using Domain.Models.Entities;

namespace Domain.Contracts.Repositories
{
    public interface ICourseRepository : IInternalRepositoryBase<Course>, IRepositoryBase<Course>
    {
        Task<Course?> GetCourseDetailsByIdAsync(int courseId, bool trackChanges = false, CancellationToken ct = default);
    }
}
