using System.Net.Http.Json;
using Microservice.Faturamento.Application.Interfaces;
using Microservice.Faturamento.Application.Models;
using Microsoft.Extensions.Logging;

namespace Microservice.Faturamento.Infrastructure.Clients;

public class PedidoConsultaClient : IPedidoConsultaClient
{
    private readonly HttpClient _http;
    private readonly ILogger<PedidoConsultaClient> _logger;

    public PedidoConsultaClient(HttpClient http, ILogger<PedidoConsultaClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<PedidoResumo?> ObterPedidoAsync(int pedidoId)
    {
        try
        {
            var resp = await _http.GetAsync($"api/pedidos/{pedidoId}");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PedidoResumo>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro consultando pedido {PedidoId}", pedidoId);
            return null;
        }
    }
}
