using Microservice.Faturamento.Domain.Entities;

namespace Microservice.Faturamento.Application.Interfaces;

public interface IFaturaRepository
{
    Task AddAsync(Fatura fatura);
    Task<Fatura?> GetByIdAsync(int id);
    Task<Fatura?> GetByPedidoIdAsync(int pedidoId);
    Task UpdateAsync(Fatura fatura);
}
