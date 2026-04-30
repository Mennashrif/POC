using Billing.Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Billing.Infrastructure.Cache;

public class PermissionCache : IPermissionCache
{
    private readonly IDistributedCache _cache;
    private readonly IRolePermissionRepository _repository;
    private readonly DistributedCacheEntryOptions _cacheOptions;

    public PermissionCache(IDistributedCache cache, IRolePermissionRepository repository, IOptions<PermissionCacheOptions> options)
    {
        _cache = cache;
        _repository = repository;
        _cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.Value.ExpirationMinutes)
        };
    }

    public async Task<List<string>> GetPermissionsAsync(string role)
    {
        var cacheKey = $"permissions:{role}";

        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached is not null)
            return JsonSerializer.Deserialize<List<string>>(cached)!;

        var permissions = await _repository.GetPermissionsByRoleAsync(role);

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(permissions),
            _cacheOptions);

        return permissions;
    }
}
