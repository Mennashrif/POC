using Booking.Application.Abstractions;
using Booking.Application.Commands;
using Booking.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CreateReservationCommandHandler).Assembly));

        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<ILocalRoomService, LocalRoomService>();

        return services;
    }
}
