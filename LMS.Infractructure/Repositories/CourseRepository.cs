using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories
{
    public class CourseRepository(ApplicationDbContext context) : RepositoryBase<Course>(context), ICourseRepository
    {
        private readonly ApplicationDbContext context = context;

        public async Task<Course?> GetCourseDetailsByIdAsync(int courseId, bool trackChanges = false)
        {
            var baseQuery = !trackChanges ? context.Courses.AsNoTracking() : context.Courses;

            return await FindByCondition(c => c.Id == courseId, trackChanges)
                .Include(c => c.Participants)
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Activities)
                .SingleAsync();
        }
    }
}
