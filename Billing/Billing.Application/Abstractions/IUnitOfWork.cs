namespace Billing.Application.Abstractions;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}
