using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using LMS.Shared.DTOs;
using LMS.Shared.DTOs.PagingDtos;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories
{
    public class CourseRepository(ApplicationDbContext context) : RepositoryBase<Course>(context), ICourseRepository
    {
        private readonly ApplicationDbContext context = context;

        public async Task<IEnumerable<Course>> FindAllByConditionAsync(CoursesQueryDto query, bool trackChanges = false, CancellationToken ct = default)
        {
            var baseQuery = !trackChanges ? context.Courses.AsNoTracking() : context.Courses;

            // ToDo: Use an OrderBy query parameter?
            var orderedCourses = query.Order == SortOrder.Ascending
                ? baseQuery.OrderBy(c => c.StartDate)
                : baseQuery.OrderByDescending(c => c.StartDate);

            return await orderedCourses
                .Skip(query.Skip)
                .Take(query.Size)
                .ToListAsync(ct);
        }

        public async Task<Course?> GetCourseDetailsByIdAsync(int courseId, bool trackChanges = false, CancellationToken ct = default)
        {
            var baseQuery = !trackChanges ? context.Courses.AsNoTracking() : context.Courses;

            return await FindByCondition(c => c.Id == courseId, trackChanges)
                .Include(c => c.Participants)
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Activities)
                .SingleAsync(ct);
        }
    }
}