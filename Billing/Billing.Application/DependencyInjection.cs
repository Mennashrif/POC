using Billing.Application.Commands;
using Billing.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Billing.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CreateBillCommandHandler).Assembly));

        services.AddScoped<IBillingService, BillingService>();

        return services;
    }
}
