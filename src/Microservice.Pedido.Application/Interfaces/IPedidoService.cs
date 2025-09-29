using Microservice.Pedido.Application.DTOs;

namespace Microservice.Pedido.Application.Interfaces;

public interface IPedidoService
{
    Task<PedidoDto> CriarAsync(PedidoCriacaoDto dto);
    Task<PedidoDto?> GetByIdAsync(int id);
    Task<IEnumerable<PedidoDto>> GetTodosAsync();
    Task<bool> CancelarAsync(int id);
}
