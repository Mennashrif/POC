using MyGateway.Authentication;
using System.IdentityModel.Tokens.Jwt;

namespace MyGateway.Endpoints;

public static class AuthEndpoints
{
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        // Returns the Keycloak login URL for the React app to redirect to
        app.MapGet("/api/account/login-url", (IKeycloakAuthService auth) =>
        {
            return Results.Ok(new { url = auth.BuildLoginUrl() });
        })
        .AllowAnonymous();

        // React /callback page sends the code here to exchange for tokens
        app.MapPost("/api/account/exchange", async (ExchangeRequest request, IKeycloakAuthService auth) =>
        {
            var result = await auth.ExchangeCodeAsync(request.Code, request.RedirectUri);

            return result is null
                ? Results.Unauthorized()
                : Results.Ok(new { accessToken = result.AccessToken, expiresIn = result.ExpiresIn });
        })
        .AllowAnonymous();

        // React sends current access token → Gateway uses stored refresh token to get a new one
        app.MapPost("/api/account/refresh", async (HttpContext ctx, IKeycloakAuthService auth) =>
        {
            var sub = ExtractSub(ctx);
            if (sub is null) return Results.Unauthorized();

            var result = await auth.RefreshAsync(sub);

            return result is null
                ? Results.Unauthorized()
                : Results.Ok(new { accessToken = result.AccessToken, expiresIn = result.ExpiresIn });
        })
        .AllowAnonymous();

        return app;
    }

    private static string? ExtractSub(HttpContext ctx)
    {
        var authHeader = ctx.Request.Headers.Authorization.ToString();
        if (!authHeader.StartsWith("Bearer ")) return null;

        var token = authHeader["Bearer ".Length..];
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token)) return null;

        return handler.ReadJwtToken(token).Subject;
    }
}

internal record ExchangeRequest(string Code, string RedirectUri);
