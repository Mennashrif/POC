namespace Login.Endpoints;

public static class AccountEndpoints
{
    public static WebApplication MapAccountEndpoints(this WebApplication app)
    {
        app.MapPost("/api/account/login", async (LoginRequest request, IHttpClientFactory factory, IConfiguration config) =>
        {
            var client = factory.CreateClient();

            var keycloak = config.GetSection("Keycloak");
            var tokenUrl = $"{keycloak["Authority"]}/protocol/openid-connect/token";

            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"]  = keycloak["ClientId"]!,
                ["username"]   = request.Username,
                ["password"]   = request.Password
            };

            var response = await client.PostAsync(tokenUrl, new FormUrlEncodedContent(form));

            if (!response.IsSuccessStatusCode)
                return Results.Unauthorized();

            var token = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>();

            return Results.Ok(token);
        })
        .WithName("Login")
        .AllowAnonymous();

        return app;
    }
}

internal record LoginRequest(string Username, string Password);

internal record KeycloakTokenResponse(
    string Access_Token,
    string Refresh_Token,
    int Expires_In,
    int Refresh_Expires_In
);
