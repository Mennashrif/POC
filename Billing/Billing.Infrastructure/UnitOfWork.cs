using Billing.Application.Abstractions;
using Billing.Infrastructure.Data;

namespace Billing.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly BillingDbContext _dbContext;

    public UnitOfWork(BillingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
