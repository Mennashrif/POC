using Microsoft.Extensions.Caching.Distributed;

namespace MyGateway.Cache;

public class RefreshTokenCache : IRefreshTokenCache
{
    private readonly IDistributedCache _cache;

    public RefreshTokenCache(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task SetAsync(string username, string refreshToken, int expirySeconds)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expirySeconds)
        };

        await _cache.SetStringAsync($"refresh:{username}", refreshToken, options);
    }

    public async Task<string?> GetAsync(string username)
    {
        return await _cache.GetStringAsync($"refresh:{username}");
    }
}
