namespace Microservice.Pedido.Application.Interfaces;

public interface IEstoqueMovimentoClient
{
    Task RegistrarSaidaAsync(int produtoId, int quantidade);
    Task RegistrarEntradaAsync(int produtoId, int quantidade);
}
