using Microservice.Faturamento.Application.Models;

namespace Microservice.Faturamento.Application.Interfaces;

public interface IPedidoConsultaClient
{
    Task<PedidoResumo?> ObterPedidoAsync(int pedidoId);
}
