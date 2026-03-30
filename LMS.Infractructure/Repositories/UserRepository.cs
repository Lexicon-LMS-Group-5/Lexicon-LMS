using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class UserRepository : RepositoryBase<ApplicationUser>, IUserRepository
{

    private readonly ApplicationDbContext Context;

    public UserRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<List<ApplicationUser>> GetAllAsync(bool trackChanges, CancellationToken ct)
    {
        var data = await GetUserRolesQuery(trackChanges).ToListAsync(ct);
        return MapUsersWithRoles(data);
    }

    public async Task<ApplicationUser?> GetByIdAsync(string id, bool trackChanges, CancellationToken ct)
    {
        var data = await GetUserRolesQuery(trackChanges)
            .Where(x => x.User.Id == id)
            .ToListAsync(ct);

        return MapUsersWithRoles(data).FirstOrDefault();
    }

    public async Task<List<ApplicationUser>> GetByCourseIdAsync(int courseId, bool trackChanges, CancellationToken ct)
    {
        var data = await GetUserRolesQuery(trackChanges)
            .Where(x => x.User.CourseId == courseId)
            .ToListAsync(ct);

        return MapUsersWithRoles(data);
    }

    private List<ApplicationUser> MapUsersWithRoles(List<(ApplicationUser User, string RoleName)> data)
    {
        return data
            .GroupBy(x => x.User.Id)
            .Select(g =>
            {
                var user = g.First().User;
                user.Roles = g.Select(x => x.RoleName).ToList();
                return user;
            })
            .ToList();
    }

    private IQueryable<(ApplicationUser User, string RoleName)> GetUserRolesQuery(bool trackChanges)
    {
        var users = trackChanges ? Context.Users : Context.Users.AsNoTracking();

        return from user in users
               join userRole in Context.UserRoles
                   on user.Id equals userRole.UserId
               join role in Context.Roles
                   on userRole.RoleId equals role.Id
               select new ValueTuple<ApplicationUser, string>(user, role.Name!);
    }
}