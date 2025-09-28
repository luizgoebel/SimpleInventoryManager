namespace Microservice.Estoque.Application.Interfaces;

public interface IEstoqueRepository : IRepository<Domain.Entities.Estoque>
{
    Task<Domain.Entities.Estoque?> GetByProdutoIdAsync(int produtoId);
}
