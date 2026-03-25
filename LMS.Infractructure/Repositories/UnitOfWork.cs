using Domain.Contracts.Repositories;
using LMS.Infractructure.Data;

namespace LMS.Infractructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext context;

    private readonly Lazy<IActivityRepository> activities;

    public IActivityRepository Activities => activities.Value;

    public UnitOfWork(ApplicationDbContext context, Lazy<IActivityRepository> activities)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));

        this.activities = activities;
    }

    public async Task CompleteAsync() => await context.SaveChangesAsync();
}