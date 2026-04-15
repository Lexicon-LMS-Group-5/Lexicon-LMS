using Domain.Models.Entities;
using LMS.Shared.DTOs;
using System.Linq.Expressions;

namespace Domain.Contracts.Repositories
{
    public interface ICourseRepository : IInternalRepositoryBase<Course>, IRepositoryBase<Course>
    {
        Task<IEnumerable<Course>> FindAllByConditionAsync(CoursesQueryDto query, bool trackChanges = false, CancellationToken ct = default);
        Task<Course?> GetCourseDetailsByConditionAsync(Expression<Func<Course, bool>> expression, bool trackChanges = false, CancellationToken ct = default);
        Task<Course?> GetCourseDetailsByIdAsync(int courseId, bool trackChanges = false, CancellationToken ct = default);
    }
}
