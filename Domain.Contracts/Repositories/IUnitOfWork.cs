namespace Domain.Contracts.Repositories;

public interface IUnitOfWork
{
    public IModuleActivityRepository Activities { get; }
    Task CompleteAsync();
}