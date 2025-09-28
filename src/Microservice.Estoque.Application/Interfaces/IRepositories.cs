namespace Microservice.Estoque.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}

public interface IEstoqueRepository : IRepository<Domain.Entities.Estoque>
{
    Task<Domain.Entities.Estoque?> GetByProdutoIdAsync(int produtoId);
}
