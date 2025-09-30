using Microservice.Recibo.Application.DTOs;

namespace Microservice.Recibo.Application.Interfaces;

public interface IReciboService
{
    Task<ReciboDto> GerarPorFaturaAsync(int faturaId, string numeroFatura, decimal valorTotal);
    Task<ReciboDto?> ObterPorFaturaAsync(int faturaId);
    Task<ReciboDto?> ObterPorIdAsync(int id);
}
