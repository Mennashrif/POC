namespace MyGateway.Authentication;

public class KeycloakOptions
{
    public const string SectionName = "Keycloak";

    public string Authority        { get; set; } = string.Empty;
    public string Audience         { get; set; } = string.Empty;
    public string ClientId         { get; set; } = string.Empty;
    public string ClientSecret     { get; set; } = string.Empty;
    public string RedirectUri      { get; set; } = string.Empty;
    public string TokenEndpointPath{ get; set; } = "/protocol/openid-connect/token";
    public string AuthEndpointPath { get; set; } = "/protocol/openid-connect/auth";
    public string Scopes           { get; set; } = "openid profile email";
}
