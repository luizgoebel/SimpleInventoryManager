using Microsoft.EntityFrameworkCore;
using Microservice.Pedido.Application.Interfaces;
using Microservice.Pedido.Infrastructure.Data.Context;

namespace Microservice.Pedido.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly PedidoDbContext _ctx;
    public PedidoRepository(PedidoDbContext ctx) => _ctx = ctx;

    public async Task AddAsync(Microservice.Pedido.Domain.Entities.Pedido pedido)
    {
        _ctx.Pedidos.Add(pedido);
        await _ctx.SaveChangesAsync();
    }

    public async Task<IEnumerable<Microservice.Pedido.Domain.Entities.Pedido>> GetAllAsync()
    {
        return await _ctx.Pedidos
            .AsNoTracking()
            .Include(p => p.Itens)
            .OrderByDescending(p => p.Id)
            .ToListAsync();
    }

    public async Task<Microservice.Pedido.Domain.Entities.Pedido?> GetByIdWithItemsAsync(int id)
    {
        return await _ctx.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task UpdateAsync(Microservice.Pedido.Domain.Entities.Pedido pedido)
    {
        _ctx.Pedidos.Update(pedido);
        await _ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(Microservice.Pedido.Domain.Entities.Pedido pedido)
    {
        _ctx.Pedidos.Remove(pedido);
        await _ctx.SaveChangesAsync();
    }
}
