using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using MyGateway.HttpClients;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRateLimiter(options =>
{
    //options.AddFixedWindowLimiter("fixed", opt =>
    //{
    //    opt.PermitLimit = 3;
    //    opt.Window = TimeSpan.FromSeconds(30);
    //    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    //    opt.QueueLimit = 1;
    //});

    options.AddSlidingWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 3;
        opt.Window = TimeSpan.FromSeconds(30);
        opt.SegmentsPerWindow = 3;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
});
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

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

app.MapReservationSummaryEndpoint();
app.MapReverseProxy();
app.Run();
