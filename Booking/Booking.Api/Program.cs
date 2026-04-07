using Booking.Api.Endpoints;
using Booking.Application;
using Booking.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapReservationEndpoints();
app.MapPaymentEndpoints();
app.UseHttpsRedirection();

app.Run();
