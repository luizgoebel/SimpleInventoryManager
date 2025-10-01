using Microservice.Faturamento.Application.Interfaces;
using Microservice.Faturamento.Domain.Entities;
using Microservice.Faturamento.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Faturamento.Infrastructure.Repositories;

public class FaturaRepository : IFaturaRepository
{
    private readonly FaturaDbContext _ctx;
    private readonly DbSet<Fatura> _set;

    public FaturaRepository(FaturaDbContext ctx)
    {
        _ctx = ctx;
        _set = ctx.Faturas;
    }

    public async Task AddAsync(Fatura fatura)
    {
        await _set.AddAsync(fatura);
        await _ctx.SaveChangesAsync();
    }

    public async Task<Fatura?> GetByIdAsync(int id)
    {
        return await _set
            .Include(f => f.Itens)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Fatura?> GetByPedidoIdAsync(int pedidoId)
    {
        return await _set
            .Include(f => f.Itens)
            .FirstOrDefaultAsync(f => f.PedidoId == pedidoId);
    }

    public async Task UpdateAsync(Fatura fatura)
    {
        _set.Update(fatura);
        await _ctx.SaveChangesAsync();
    }
}
