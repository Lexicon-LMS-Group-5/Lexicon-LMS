using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class UserRepository : RepositoryBase<ApplicationUser>, IUserRepository
{
    public UserRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<List<ApplicationUser>> GetAllAsync(bool trackChanges, CancellationToken ct) =>
        await FindAll(trackChanges).ToListAsync();

    public async Task<ApplicationUser?> GetByIdAsync(string id, bool trackChanges, CancellationToken ct) =>
        await FindByCondition(a => a.Id == id, trackChanges)
            .FirstOrDefaultAsync();

    public async Task<List<ApplicationUser>> GetActivitiesByCourseIdAsync(int courseId, bool trackChanges, CancellationToken ct) =>
        await FindByCondition(a => a.CourseId == courseId)
            .ToListAsync();
}
