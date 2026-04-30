namespace Billing.Application.Abstractions;

public interface IPermissionCache
{
    Task<List<string>> GetPermissionsAsync(string role);
}
