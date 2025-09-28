using Microservice.Produto.Application.DTOs;

namespace Microservice.Produto.Application.Interfaces;

public interface IProdutoService
{
    Task<ProdutoDto> CriarProdutoAsync(ProdutoCriacaoDto dto);
    Task<ProdutoDto?> GetProdutoByIdAsync(int id);
    Task<IEnumerable<ProdutoDto>> GetTodosAsync();
    Task<ProdutoDto?> AtualizarAsync(int id, ProdutoAtualizacaoDto dto);
    Task<bool> DeletarAsync(int id);
}
