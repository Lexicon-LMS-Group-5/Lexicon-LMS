namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    public IActivityRepository Activities { get; }
    Task CompleteAsync();
}