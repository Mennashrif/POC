using Billing.Application.Abstractions;
using Billing.Application.Services;
using Billing.Infrastructure.Cache;
using Billing.Infrastructure.Data;
using Billing.Infrastructure.Messaging;
using Billing.Infrastructure.Repositories;
using Billing.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Billing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BillingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("BillingDb")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IBillRepository, BillRepository>();
        services.AddScoped<IProcessedEventRepository, ProcessedEventRepository>();
        services.AddScoped<IBillingFileRepository, BillingFileRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IFileStorage, LocalFileStorage>();
        services.AddScoped<IDataExtractor, DataExtractor>();
        services.AddSingleton<IYaraScanner, YaraScanner>();
        services.AddScoped<IFileValidator, FileValidator>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        services.Configure<PermissionCacheOptions>(configuration.GetSection("PermissionCache"));
        services.AddScoped<IPermissionCache, PermissionCache>();

        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));
        services.AddHostedService<ReservationCheckedInConsumer>();

        return services;
    }
}
