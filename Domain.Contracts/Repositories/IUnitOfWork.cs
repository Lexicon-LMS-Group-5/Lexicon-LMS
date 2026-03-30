namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    IActivityRepository Activities { get; }
    ICourseRepository Courses { get; }
    IModuleRepository Modules { get; }

    IUserRepository Users { get; }
    Task CompleteAsync();
}