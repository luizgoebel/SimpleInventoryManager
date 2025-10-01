using Microservice.Pedido.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Microservice.Pedido.Application.Clients;

public class EstoqueMovimentoClient : IEstoqueMovimentoClient
{
    private readonly HttpClient _http;
    private readonly ILogger<EstoqueMovimentoClient> _logger;

    public EstoqueMovimentoClient(HttpClient http, ILogger<EstoqueMovimentoClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task RegistrarSaidaAsync(int produtoId, int quantidade)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/estoque/saida", new { produtoId, quantidade });
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Falha ao registrar saída de estoque Produto {ProdutoId}", produtoId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao chamar serviço de estoque (saída)");
        }
    }

    public async Task RegistrarEntradaAsync(int produtoId, int quantidade)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/estoque/entrada", new { produtoId, quantidade });
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Falha ao registrar entrada de estoque Produto {ProdutoId}", produtoId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao chamar serviço de estoque (entrada)");
        }
    }
}
