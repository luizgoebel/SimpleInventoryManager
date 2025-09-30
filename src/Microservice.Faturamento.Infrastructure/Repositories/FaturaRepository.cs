using Microservice.Faturamento.Application.Interfaces;
using Microservice.Faturamento.Domain.Entities;

namespace Microservice.Faturamento.Infrastructure.Repositories;

public class FaturaRepository : IFaturaRepository
{
    private static readonly List<Fatura> _db = new();

    public Task AddAsync(Fatura fatura)
    {
        fatura.Id = _db.Count + 1;
        _db.Add(fatura);
        return Task.CompletedTask;
    }

    public Task<Fatura?> GetByIdAsync(int id)
        => Task.FromResult(_db.FirstOrDefault(f => f.Id == id));

    public Task<Fatura?> GetByPedidoIdAsync(int pedidoId)
        => Task.FromResult(_db.FirstOrDefault(f => f.PedidoId == pedidoId));

    public Task UpdateAsync(Fatura fatura) => Task.CompletedTask;
}
