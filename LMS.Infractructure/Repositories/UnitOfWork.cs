using Domain.Contracts.Repositories;
using LMS.Infractructure.Data;

namespace LMS.Infractructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext context;

    public IActivityRepository Activities { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));

        Activities = new ActivityRepository(context);
    }

    public async Task CompleteAsync() => await context.SaveChangesAsync();
}
