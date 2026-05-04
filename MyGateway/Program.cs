using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using MyGateway.Authentication;
using MyGateway.Cache;
using MyGateway.Endpoints;
using MyGateway.HttpClients;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddSlidingWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 3;
        opt.Window = TimeSpan.FromSeconds(30);
        opt.SegmentsPerWindow = 3; 
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
});

// Keycloak JWT validation
builder.Services.Configure<KeycloakOptions>(builder.Configuration.GetSection(KeycloakOptions.SectionName));

var keycloakSettings = builder.Configuration.GetSection(KeycloakOptions.SectionName);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakSettings["Authority"];
        options.RequireHttpsMetadata = false;
        options.Audience = keycloakSettings["Audience"];
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorization();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Redis cache for refresh tokens
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
builder.Services.AddScoped<IRefreshTokenCache, RefreshTokenCache>();

// Keycloak auth service
builder.Services.AddHttpClient<IKeycloakAuthService, KeycloakAuthService>();

// Downstream service HTTP clients
builder.Services.AddHttpClient<IBookingClient, BookingClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Services:Booking"]!));

builder.Services.AddHttpClient<IBillingClient, BillingClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Services:Billing"]!));

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// Auth endpoints before YARP
app.MapAuthEndpoints();
app.MapReservationSummaryEndpoint();
app.MapReverseProxy();
app.Run();
