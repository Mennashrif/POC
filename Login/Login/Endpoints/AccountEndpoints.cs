using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Login.Endpoints
{
    public static class AccountEndpoints
    {
        public static WebApplication MapAccountEndpoints(this WebApplication app)
        {
            app.MapPost("/api/account/login", (LoginRequest request, IConfiguration config) =>
            {
                if (request.Username == "admin" && request.Password == "password")
                {
                    var token = GenerateJwtToken(request.Username, config);
                    return Results.Ok(new LoginResponse(Token: token));
                }
                return Results.Unauthorized();
            })
            .WithName("Login");
            return app;
        }

        private static string GenerateJwtToken(string username, IConfiguration config)
        {
            var jwtSettings = config.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]!;
            var issuer = jwtSettings["Issuer"]!;
            var audience = jwtSettings["Audience"]!;
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"]!);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    internal record LoginResponse(string Token);
    internal record LoginRequest(string Username, string Password);
}
