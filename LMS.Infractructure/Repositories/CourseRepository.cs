using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;

namespace LMS.Infractructure.Repositories
{
    public class CourseRepository(ApplicationDbContext context) : RepositoryBase<Course>(context), ICourseRepository
    {
    }
}
