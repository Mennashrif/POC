using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoomManagement.Application.Abstractions;
using RoomManagement.Infrastructure.Data;
using RoomManagement.Infrastructure.Jobs;
using RoomManagement.Infrastructure.Messaging;
using RoomManagement.Infrastructure.Repositories;

namespace RoomManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RoomManagementDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("RoomManagementDb")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IProcessedEventRepository, ProcessedEventRepository>();

        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));
        services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();
        services.AddHostedService<ReservationConsumer>();

        services.AddScoped<OutboxProcessorJob>();

        return services;
    }
}
