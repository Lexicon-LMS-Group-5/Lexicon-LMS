using Domain.Contracts.Repositories;
using Domain.Models.Entities;
using LMS.Infractructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infractructure.Repositories;

public class UserRepository(ApplicationDbContext context) : RepositoryBase<ApplicationUser>(context), IUserRepository
{

    private readonly ApplicationDbContext _context = context;

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

    private static List<ApplicationUser> MapUsersWithRoles(List<UserRoleResult> data)
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

    private IQueryable<UserRoleResult> GetUserRolesQuery(bool trackChanges)
    {
        var users = trackChanges ? _context.Users.Include(u => u.Course)
                                 : _context.Users.Include(u => u.Course).AsNoTracking();

        return from user in users
               join userRole in _context.UserRoles
                   on user.Id equals userRole.UserId into ur
               from userRole in ur.DefaultIfEmpty()
               join role in _context.Roles
                   on userRole.RoleId equals role.Id into r
               from role in r.DefaultIfEmpty()
               select new UserRoleResult
               {
                   User = user,
                   RoleName = role != null ? role.Name! : string.Empty
               };
    }

    private class UserRoleResult
    {
        public ApplicationUser User { get; set; } = default!;
        public string RoleName { get; set; } = string.Empty;
    }
}