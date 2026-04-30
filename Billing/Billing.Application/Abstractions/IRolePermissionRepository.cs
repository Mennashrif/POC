namespace Billing.Application.Abstractions;

public interface IRolePermissionRepository
{
    Task<List<string>> GetPermissionsByRoleAsync(string role);
}
