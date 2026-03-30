using LMS.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Service.Contracts
{
    public interface IModuleService
    {
        public Task<ModuleReadDto> GetModuleDetailsByIdAsync(int moduleId, bool trackChanges = false, CancellationToken ct = default);
        public Task<List<ModuleReadDto>> GetModulesByCourseIdAsync(int courseId, bool trackChanges = false, CancellationToken ct = default);
    }
}
