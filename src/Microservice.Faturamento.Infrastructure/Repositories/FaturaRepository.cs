using Microservice.Faturamento.Application.Interfaces;
using Microservice.Faturamento.Domain.Entities;
using Microservice.Faturamento.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Faturamento.Infrastructure.Repositories;

public class FaturaRepository : IFaturaRepository
{
    private static readonly List<Fatura> _db = new();
    protected readonly FaturaDbContext _ctx;
    protected readonly DbSet<Fatura> _set;

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

    public async Task UpdateAsync(Fatura fatura)
    {
        _set.Update(fatura);
        await _ctx.SaveChangesAsync();
        await Task.CompletedTask;
    }
}
