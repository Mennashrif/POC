using Billing.Domain.Abstractions;

namespace Billing.Domain.Models;

public class RolePermission : BaseEntity<Guid>
{
    public string Role { get; private set; }
    public string Permission { get; private set; }

    private RolePermission() : base(Guid.Empty) { }

    public RolePermission(string role, string permission) : base(Guid.NewGuid())
    {
        Role = role;
        Permission = permission;
    }
}
