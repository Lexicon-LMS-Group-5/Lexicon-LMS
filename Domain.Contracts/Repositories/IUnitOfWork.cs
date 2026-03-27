namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    IActivityRepository Activities { get; }
    ICourseRepository Courses { get; }
    IModuleRepository ModuleRepository { get; }
    Task CompleteAsync();
}
