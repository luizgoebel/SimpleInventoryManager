using Microservice.Faturamento.Application.DTOs;

namespace Microservice.Faturamento.Application.Interfaces;

public interface IFaturamentoService
{
    Task<FaturaCriacaoResultadoDto> FaturarPedidoAsync(int pedidoId);
    Task<FaturaDto?> ObterPorIdAsync(int id);
    Task<FaturaDto?> ObterPorPedidoAsync(int pedidoId);
}
