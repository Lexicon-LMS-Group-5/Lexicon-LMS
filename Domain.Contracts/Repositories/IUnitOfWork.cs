namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    IActivityRepository Activities { get; }
    ICourseRepository CourseRepository { get; }
    IModuleRepository ModuleRepository { get; }
    Task CompleteAsync();
}