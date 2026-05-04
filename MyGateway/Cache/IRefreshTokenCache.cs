namespace MyGateway.Cache;

public interface IRefreshTokenCache
{
    Task SetAsync(string username, string refreshToken, int expirySeconds);
    Task<string?> GetAsync(string username);
}
