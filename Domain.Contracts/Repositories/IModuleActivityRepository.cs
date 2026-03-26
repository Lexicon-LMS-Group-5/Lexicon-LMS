using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface IActivityRepository : IRepositoryBase<Activity>, IInternalRepositoryBase<Activity>
{
    Task<List<Activity>> GetAllAsync(bool trackChanges, CancellationToken ct);
    Task<Activity?> GetByIdAsync(int id, bool trackChanges, CancellationToken ct);
    Task<List<Activity>> GetActivitiesByModuleIdAsync(int moduleId, bool trackChanges, CancellationToken ct);
    Task<List<Activity>> GetActivitiesByTypeIdAsync(int typeId, bool trackChanges, CancellationToken ct);
    Task<List<Activity>> GetActivitiesByDateRangeAsync(DateTime startDate, DateTime endDate, bool trackChanges, CancellationToken ct);
}
