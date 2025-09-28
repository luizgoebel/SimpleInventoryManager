namespace Microservice.Produto.Application.Interfaces;

public interface IEstoqueClient
{
    Task CriarEstoqueInicialAsync(int produtoId);
}
