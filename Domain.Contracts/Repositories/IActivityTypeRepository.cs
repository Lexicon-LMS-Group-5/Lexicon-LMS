using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface IActivityTypeRepository : IRepositoryBase<ActivityType>, IInternalRepositoryBase<ActivityType>
{
    Task<List<ActivityType>> GetAllAsync(bool trackChanges, CancellationToken ct);

    Task<ActivityType?> GetByIdAsync(int id, bool trackChanges, CancellationToken ct);
}