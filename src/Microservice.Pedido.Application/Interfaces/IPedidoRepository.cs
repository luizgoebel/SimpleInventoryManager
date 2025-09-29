namespace Microservice.Pedido.Application.Interfaces;

public interface IPedidoRepository
{
    Task AddAsync(Microservice.Pedido.Domain.Entities.Pedido pedido);
    Task<Microservice.Pedido.Domain.Entities.Pedido?> GetByIdWithItemsAsync(int id);
    Task<IEnumerable<Microservice.Pedido.Domain.Entities.Pedido>> GetAllAsync();
    Task UpdateAsync(Microservice.Pedido.Domain.Entities.Pedido pedido);
    Task DeleteAsync(Microservice.Pedido.Domain.Entities.Pedido pedido);
}
