namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    ICourseRepository Courses { get; }
    IModuleRepository Modules { get; }
    IActivityRepository Activities { get; }
    IActivityTypeRepository ActivityTypes { get; }
    IUserRepository Users { get; }

    Task CompleteAsync(CancellationToken ct = default);
}
