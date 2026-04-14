using Booking.Application.Abstractions;
using Booking.Infrastructure.Data;
using Booking.Infrastructure.Jobs;
using Booking.Infrastructure.Messaging;
using Booking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BookingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("BookingDb")));

        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));
        services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();

        services.AddScoped<OutboxProcessorJob>();
        return services;
    }
}
