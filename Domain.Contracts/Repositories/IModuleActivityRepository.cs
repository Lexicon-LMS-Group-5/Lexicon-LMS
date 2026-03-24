using Domain.Models.Entities;
using LMS.Shared.DTOs;

namespace Domain.Contracts.Repositories;

public interface IModuleActivityRepository : IRepositoryBase<ModuleActivity>, IInternalRepositoryBase<ModuleActivity>
{
    Task<List<ModuleActivity>> GetAllAsync(bool trackChanges, CancellationToken ct);
    Task<ModuleActivity?> GetByIdAsync(int id, bool trackChanges, CancellationToken ct);
    Task<List<ModuleActivity>> GetActivitiesByModuleIdAsync(int moduleId, bool trackChanges, CancellationToken ct);
    Task<List<ModuleActivity>> GetActivitiesByTypeIdAsync(int typeId, bool trackChanges, CancellationToken ct);
    Task<List<ModuleActivity>> GetActivitiesByDateRangeAsync(DateTime startDate, DateTime endDate, bool trackChanges, CancellationToken ct);
}
