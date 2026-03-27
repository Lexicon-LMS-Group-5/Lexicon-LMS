using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infractructure.Repositories
{
    public class ModuleRepository(ApplicationDbContext context) : RepositoryBase<Module>(context), IModuleRepository
    {
        private readonly ApplicationDbContext context = context;

        public Task<Module?> GetModuleDetailsByIdAsync(int moduleId, bool trackChanges = false, CancellationToken ct = default)
        {
            var baseQuery = !trackChanges ? context.Modules.AsNoTracking() : context.Modules;
            return FindByCondition(m => m.Id == moduleId, trackChanges)
                .Include(m => m.Activities)
                .SingleOrDefaultAsync(ct);
        }
        public Task<List<Module>> GetModulesByCourseIdAsync(int courseId, bool trackChanges = false, CancellationToken ct = default)
        {
            var baseQuery = !trackChanges ? context.Modules.AsNoTracking() : context.Modules;
            return FindByCondition(m => m.CourseId == courseId, trackChanges)
                .Include(m => m.Activities)
                .ToListAsync(ct);
        }
    }
}
