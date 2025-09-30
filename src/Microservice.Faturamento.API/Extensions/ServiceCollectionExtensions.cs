using Microservice.Faturamento.Application.Interfaces;
using Microservice.Faturamento.Application.Services;
using Microservice.Faturamento.Infrastructure.Clients;
using Microservice.Faturamento.Infrastructure.Repositories;

namespace Microservice.Faturamento.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFaturamentoModule(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddScoped<IFaturamentoService, FaturamentoService>();
        services.AddSingleton<IFaturaRepository, FaturaRepository>();

        services.AddHttpClient<IPedidoConsultaClient, PedidoConsultaClient>(c =>
        {
            var url = cfg["ServiceUrls:PedidoAPI"]; if (!string.IsNullOrWhiteSpace(url)) c.BaseAddress = new Uri(url);
        });
        services.AddHttpClient<IReciboEmissaoClient, ReciboEmissaoClient>(c =>
        {
            var url = cfg["ServiceUrls:ReciboAPI"]; if (!string.IsNullOrWhiteSpace(url)) c.BaseAddress = new Uri(url);
        });
        return services;
    }
}
