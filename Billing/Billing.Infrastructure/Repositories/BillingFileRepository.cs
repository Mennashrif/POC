using Billing.Application.Abstractions;
using Billing.Domain.Models;
using Billing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Billing.Infrastructure.Repositories;

public class BillingFileRepository : IBillingFileRepository
{
    private readonly BillingDbContext _context;

    public BillingFileRepository(BillingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(BillingFile file)
    {
        await _context.BillingFiles.AddAsync(file);
    }

    public async Task<BillingFile?> GetByIdAsync(Guid id)
    {
        return await _context.BillingFiles.FindAsync(id);
    }
}
