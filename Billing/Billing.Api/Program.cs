using Billing.Api.Authentication;
using Billing.Api.Endpoints;
using Billing.Application;
using Billing.Infrastructure;
using Billing.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Keycloak JWT validation + permission enrichment
builder.Services.AddScoped<KeycloakTokenEnricher>();

var keycloakSettings = builder.Configuration.GetSection("Keycloak");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakSettings["Authority"];
        options.Audience = keycloakSettings["Audience"];
        options.RequireHttpsMetadata = false;
        options.EventsType = typeof(KeycloakTokenEnricher);
    });

// Permission-based authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("read:bills",     policy => policy.RequireClaim("permission", "read:bills"));
    options.AddPolicy("write:bills",    policy => policy.RequireClaim("permission", "write:bills"));
    options.AddPolicy("upload:files",   policy => policy.RequireClaim("permission", "upload:files"));
    options.AddPolicy("download:files", policy => policy.RequireClaim("permission", "download:files"));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapBillingEndpoints();
app.MapFileEndpoints();
app.UseHttpsRedirection();

app.Run();
