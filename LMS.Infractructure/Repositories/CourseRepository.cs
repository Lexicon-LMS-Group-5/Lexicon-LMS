using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using System.Linq.Expressions;

namespace LMS.Infractructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        public void Create(Course entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Course entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Course> FindAll(bool trackChanges = false)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Course> FindByCondition(Expression<Func<Course, bool>> expression, bool trackChanges = false)
        {
            throw new NotImplementedException();
        }

        public void Update(Course entity)
        {
            throw new NotImplementedException();
        }
    }
}
