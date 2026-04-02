using Domain.Models.Entities;

namespace Domain.Contracts.Repositories;

public interface IUserRepository : IRepositoryBase<ApplicationUser>, IInternalRepositoryBase<ApplicationUser>
{
    Task<List<ApplicationUser>> GetAllAsync(bool trackChanges, CancellationToken ct);
    Task<ApplicationUser?> GetByIdAsync(string id, bool trackChanges, CancellationToken ct);
    Task<List<ApplicationUser>> GetByCourseIdAsync(int moduleId, bool trackChanges, CancellationToken ct);
}
