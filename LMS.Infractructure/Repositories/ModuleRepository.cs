using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infractructure.Repositories
{
    public class ModuleRepository(ApplicationDbContext context) 
        : RepositoryBase<Module>(context), IModuleRepository
    {
        private readonly ApplicationDbContext context = context;

        public async Task<Module?> GetModuleDetailsByIdAsync(
            int moduleId, 
            bool trackChanges = false, 
            CancellationToken ct = default)
        {
            return await FindByCondition(m => m.Id == moduleId, trackChanges)
                .Include(m => m.Activities)
                .SingleAsync(ct);
        }
        public async Task<List<Module>> GetModulesByCourseIdAsync(
            int courseId, 
            bool trackChanges = false, 
            CancellationToken ct = default)
        {
            return await FindByCondition(m => m.CourseId == courseId, trackChanges)
                .Include(m => m.Activities)
                .ToListAsync(ct);
        }
    }
}
