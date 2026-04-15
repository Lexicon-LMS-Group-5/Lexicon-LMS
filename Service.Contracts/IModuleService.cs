using LMS.Shared.DTOs;

namespace Service.Contracts
{
    public interface IModuleService
    {
        public Task<ModuleReadDto> GetModuleDetailsByIdAsync(
            int moduleId,
            bool trackChanges = false,
            CancellationToken ct = default);
        public Task<List<ModuleReadDto>> GetModulesByCourseIdAsync(
            int courseId,
            bool trackChanges = false,
            CancellationToken ct = default);
        public Task<ModuleReadDto> CreateModuleAsync(ModuleUpsertDto dto, CancellationToken ct = default);
        public Task<ModuleReadDto> UpdateModuleAsync(int id, ModuleUpsertDto dto, CancellationToken ct = default);
        public Task DeleteModuleAsync(int moduleId, ModuleCourseIdDto courseId, CancellationToken ct = default);
    }
}
