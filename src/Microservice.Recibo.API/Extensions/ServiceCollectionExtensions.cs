using Microservice.Recibo.Application.Interfaces;
using Microservice.Recibo.Application.Services;
using Microservice.Recibo.Infrastructure.Repositories;

namespace Microservice.Recibo.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReciboModule(this IServiceCollection services)
    {
        services.AddScoped<IReciboService, ReciboService>();
        services.AddSingleton<IReciboRepository, ReciboRepository>();
        return services;
    }
}
