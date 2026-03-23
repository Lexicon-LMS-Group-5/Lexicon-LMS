using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LMS.Infractructure.Repositories
{
    public class CourseRepository(ApplicationDbContext context) : RepositoryBase<Course>(context), ICourseRepository
    {
        ApplicationDbContext context = context;
        public async Task<bool> CourseExistsAsync(int courseId)
        {
            return await context.Courses.FindAsync(courseId) is not null;
        }

        public async Task<Course?> GetCourseDetailsByConditionAsync(Expression<Func<Course, bool>> expression, bool trackChanges = false)
        {
            var baseQuery = !trackChanges ? context.Courses.AsNoTracking() : context.Courses;

            return await FindByCondition(expression, trackChanges)
                .Include(c => c.Participants)
                .Include(c => c.Modules)
                .SingleAsync();
        }
    }
}
