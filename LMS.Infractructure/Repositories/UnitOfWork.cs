using Domain.Contracts.Repositories;
using LMS.Infractructure.Data;

namespace LMS.Infractructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly Lazy<ICourseRepository> courseRepository;
    public ICourseRepository Courses => courseRepository.Value;

    private readonly Lazy<IModuleRepository> moduleRepository;
    public IModuleRepository Modules => moduleRepository.Value;

    private readonly Lazy<IActivityRepository> activities;
    public IActivityRepository Activities => activities.Value;

    private readonly Lazy<IUserRepository> users;
    public IUserRepository Users => users.Value;

    private readonly ApplicationDbContext context;

    public UnitOfWork(
        ApplicationDbContext context,
        Lazy<ICourseRepository> courseRepository,
        Lazy<IModuleRepository> moduleRepository,
        Lazy<IActivityRepository> activities,
        Lazy<IUserRepository> users)

    {
        this.courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
        this.moduleRepository = moduleRepository ?? throw new ArgumentNullException(nameof(moduleRepository));
        this.activities = activities ?? throw new ArgumentNullException(nameof(activities));
        this.users = users ?? throw new ArgumentNullException(nameof(users));
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task CompleteAsync() => await context.SaveChangesAsync();
}