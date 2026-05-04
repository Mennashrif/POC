namespace MyGateway.Authentication;

public interface IKeycloakAuthService
{
    string BuildLoginUrl();
    Task<TokenResult?> ExchangeCodeAsync(string code, string redirectUri);
    Task<TokenResult?> RefreshAsync(string sub);
}

public record TokenResult(string AccessToken, int ExpiresIn);
