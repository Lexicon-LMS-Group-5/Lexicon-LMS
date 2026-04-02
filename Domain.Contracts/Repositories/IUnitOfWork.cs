namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    IActivityRepository Activities { get; }
    ICourseRepository CourseRepository { get; }
    Task CompleteAsync();
}