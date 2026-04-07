using Billing.Application.Abstractions;
using Billing.Infrastructure.Data;
using Billing.Infrastructure.Messaging;
using Billing.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Billing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BillingDbContext>(options =>
            options.UseInMemoryDatabase("BillingDb"));

        services.AddScoped<IBillRepository, BillRepository>();

        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));
        services.AddHostedService<ReservationCheckedInConsumer>();

        return services;
    }
}
