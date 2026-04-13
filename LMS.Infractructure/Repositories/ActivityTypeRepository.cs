using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class ActivityTypeRepository : RepositoryBase<ActivityType>, IActivityTypeRepository
{
    public ActivityTypeRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<List<ActivityType>> GetAllAsync(bool trackChanges, CancellationToken ct) =>
        await FindAll(trackChanges).ToListAsync(ct);

    public async Task<ActivityType?> GetByIdAsync(int id, bool trackChanges, CancellationToken ct) =>
        await FindByCondition(x => x.Id == id, trackChanges).FirstOrDefaultAsync();
}