using Microsoft.Extensions.Options;
using MyGateway.Cache;
using System.IdentityModel.Tokens.Jwt;

namespace MyGateway.Authentication;

public class KeycloakAuthService : IKeycloakAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IRefreshTokenCache _refreshTokenCache;
    private readonly KeycloakOptions _options;
    private readonly string _tokenEndpoint;

    public KeycloakAuthService(HttpClient httpClient, IRefreshTokenCache refreshTokenCache, IOptions<KeycloakOptions> options, ILogger<KeycloakAuthService> logger)
    {
        _httpClient = httpClient;
        _refreshTokenCache = refreshTokenCache;
        _options = options.Value;
        _tokenEndpoint = $"{_options.Authority}{_options.TokenEndpointPath}";
    }

    public string BuildLoginUrl()
    {
        var query = new Dictionary<string, string>
        {
            ["response_type"] = "code",
            ["client_id"]     = _options.ClientId,
            ["redirect_uri"]  = _options.RedirectUri,
            ["scope"]         = _options.Scopes
        };

        var queryString = string.Join("&", query.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
        return $"{_options.Authority}{_options.AuthEndpointPath}?{queryString}";
    }

    public async Task<TokenResult?> ExchangeCodeAsync(string code, string redirectUri)
    {
        var form = new Dictionary<string, string>
        {
            ["grant_type"]   = "authorization_code",
            ["client_id"]    = _options.ClientId,
            ["code"]         = code,
            ["redirect_uri"] = redirectUri,
            ["client_secret"] = _options.ClientSecret
        };


        return await RequestTokenAsync(form);
    }

    public async Task<TokenResult?> RefreshAsync(string sub)
    {
        var refreshToken = await _refreshTokenCache.GetAsync(sub);
        if (refreshToken is null) return null;

        var form = new Dictionary<string, string>
        {
            ["grant_type"]    = "refresh_token",
            ["client_id"]     = _options.ClientId,
            ["refresh_token"] = refreshToken,
            ["client_secret"] = _options.ClientSecret
        };

        return await RequestTokenAsync(form);
    }

    private async Task<TokenResult?> RequestTokenAsync(Dictionary<string, string> form)
    {
        var response = await _httpClient.PostAsync(_tokenEndpoint, new FormUrlEncodedContent(form));
        if (response.IsSuccessStatusCode)
        {
            var keycloakToken = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>();
            if (keycloakToken is null) return null;

            var sub = ExtractSub(keycloakToken.Access_Token);
            if (sub is not null)
                await _refreshTokenCache.SetAsync(sub, keycloakToken.Refresh_Token, keycloakToken.Refresh_Expires_In);

            return new TokenResult(keycloakToken.Access_Token, keycloakToken.Expires_In);
        }

        return null;

    }

    private static string? ExtractSub(string accessToken)
    {
        // Decode JWT without validation just to read the sub claim
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(accessToken)) return null;

        var jwt = handler.ReadJwtToken(accessToken);
        return jwt.Subject;
    }
}

