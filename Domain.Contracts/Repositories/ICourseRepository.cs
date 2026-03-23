using Domain.Models.Entities;
using System.Linq.Expressions;

namespace Domain.Contracts.Repositories
{
    public interface ICourseRepository : IInternalRepositoryBase<Course>, IRepositoryBase<Course>
    {
        Task<bool> CourseExistsAsync(int courseId);

        Task<Course?> GetCourseDetailsByConditionAsync(Expression<Func<Course, bool>> expression, bool trackChanges = false);
    }
}
