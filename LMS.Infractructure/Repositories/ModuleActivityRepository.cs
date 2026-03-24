using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class ActivityRepository : RepositoryBase<Activity>, IActivityRepository
{
    public ActivityRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<List<Activity>> GetAllAsync(bool trackChanges, CancellationToken ct) =>
        await FindAll(trackChanges).ToListAsync();

    public async Task<Activity?> GetByIdAsync(int id, bool trackChanges, CancellationToken ct) =>
        await FindByCondition(x => x.Id == id, trackChanges)
            .FirstOrDefaultAsync();
    public async Task<List<Activity>> GetActivitiesByModuleIdAsync(int moduleId, bool trackChanges, CancellationToken ct) =>
       await FindByCondition(a => a.ModuleId == moduleId, trackChanges).ToListAsync();
    public async Task<List<Activity>> GetActivitiesByTypeIdAsync(int typeId, bool trackChanges, CancellationToken ct) =>
        await FindByCondition(a => a.ActivityTypeId == typeId).ToListAsync();
    public async Task<List<Activity>> GetActivitiesByDateRangeAsync(DateTime startDate, DateTime endDate, bool trackChanges, CancellationToken ct) =>
        await FindByCondition(a => a.StartDate >= startDate && a.EndDate <= endDate).ToListAsync();
}
