namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    IActivityRepository Activities { get; }
    ICourseRepository Courses { get; }
    Task CompleteAsync();
}