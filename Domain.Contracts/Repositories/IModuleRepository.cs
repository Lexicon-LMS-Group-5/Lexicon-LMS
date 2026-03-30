using Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Contracts.Repositories
{
    public interface IModuleRepository : IInternalRepositoryBase<Module>, IRepositoryBase<Module>
    {
        public Task<Module?> GetModuleDetailsByIdAsync(
            int moduleId, 
            bool trackChanges = false, 
            CancellationToken ct = default);
        public Task<List<Module>> GetModulesByCourseIdAsync(
            int courseId, 
            bool trackChanges = false, 
            CancellationToken ct = default);
    }
}
