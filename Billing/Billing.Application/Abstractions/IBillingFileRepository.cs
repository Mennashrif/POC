using Billing.Domain.Models;

namespace Billing.Application.Abstractions;

public interface IBillingFileRepository
{
    Task AddAsync(BillingFile file);
    Task<BillingFile?> GetByIdAsync(Guid id);
}
