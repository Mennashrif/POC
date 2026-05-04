using Billing.Application.Abstractions;
using Billing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Billing.Infrastructure.Repositories;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly BillingDbContext _context;

    public RolePermissionRepository(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<List<string>> GetPermissionsByRoleAsync(string role)
    {
        return await _context.RolePermissions
            .Where(rp => rp.Role == role)
            .Select(rp => rp.Permission)
            .ToListAsync();
    }
}
