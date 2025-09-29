using System.Net.Http.Json;
using Microservice.Produto.Application.Interfaces;

namespace Microservice.Produto.Application.Services;

public class EstoqueClient : IEstoqueClient
{
    private readonly HttpClient _httpClient;
    public EstoqueClient(HttpClient httpClient)
    {
        this._httpClient = httpClient;
    }

    public async Task CriarEstoqueInicialAsync(int produtoId)
    {
        var estoqueInicial = new { ProdutoId = produtoId, Quantidade = 0 };
        try
        {
            var response = await this._httpClient.PostAsJsonAsync("api/estoques/entrada", estoqueInicial);
            response.EnsureSuccessStatusCode();
        }
        catch
        {
            // Logar falha de comunicação (omitted)
        }
    }
}
