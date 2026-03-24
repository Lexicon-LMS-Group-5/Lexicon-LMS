using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infractructure.Repositories
{

    public class ModuleRepository(ApplicationDbContext context)
        : RepositoryBase<Module>(context), IModuleRepository
    {
        public IQueryable<Module> GetModules(int courseId, bool trackChanges) =>
            FindByCondition(m => m.CourseId == courseId, trackChanges);
    }
}
