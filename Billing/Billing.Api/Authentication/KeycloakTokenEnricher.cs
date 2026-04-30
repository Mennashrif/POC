using Billing.Application.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Text.Json;

namespace Billing.Api.Authentication;

public class KeycloakTokenEnricher : JwtBearerEvents
{
    private readonly IPermissionCache _permissionCache;

    public KeycloakTokenEnricher(IPermissionCache permissionCache)
    {
        _permissionCache = permissionCache;
    }

    public override async Task TokenValidated(TokenValidatedContext ctx)
    {
        var realmAccessJson = ctx.Principal?.FindFirst("realm_access")?.Value;
        if (realmAccessJson is null) return;

        var realmAccess = JsonSerializer.Deserialize<JsonElement>(realmAccessJson);
        if (!realmAccess.TryGetProperty("roles", out var rolesElement)) return;

        var roles = rolesElement.EnumerateArray()
            .Select(r => r.GetString())
            .Where(r => r is not null)
            .ToList();

        var allPermissions = new List<string>();
        foreach (var role in roles)
        {
            var permissions = await _permissionCache.GetPermissionsAsync(role!);
            allPermissions.AddRange(permissions);
        }

        var permissionClaims = allPermissions
            .Distinct()
            .Select(p => new Claim("permission", p));

        ctx.Principal!.AddIdentity(new ClaimsIdentity(permissionClaims));
    }
}
