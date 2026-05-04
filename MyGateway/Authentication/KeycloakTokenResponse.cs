using System.Text.Json.Serialization;

namespace MyGateway.Authentication;

public sealed record KeycloakTokenResponse(
    [property: JsonPropertyName("access_token")]       string Access_Token,
    [property: JsonPropertyName("refresh_token")]      string Refresh_Token,
    [property: JsonPropertyName("expires_in")]         int    Expires_In,
    [property: JsonPropertyName("refresh_expires_in")] int    Refresh_Expires_In
);
