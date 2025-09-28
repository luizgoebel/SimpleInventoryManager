using Microservice.Estoque.Application.DTOs;

namespace Microservice.Estoque.Application.Interfaces;

public interface IEstoqueService
{
    Task<EstoqueDto?> GetPorProdutoIdAsync(int produtoId);
    Task<bool> EntradaAsync(MovimentoEstoqueDto dto);
    Task<bool> SaidaAsync(MovimentoEstoqueDto dto);
    Task<bool> CriarInicialSeNaoExisteAsync(int produtoId);
}
