using Microservice.Recibo.Application.Interfaces;
using Microservice.Recibo.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Recibo.Infrastructure.Repositories;

public class ReciboRepository : IReciboRepository
{
    private readonly ReciboDbContext _ctx;

    public ReciboRepository(ReciboDbContext ctx) => _ctx = ctx;

    public async Task AddAsync(Domain.Entities.Recibo recibo)
    {
        await _ctx.Recibos.AddAsync(recibo);
        await _ctx.SaveChangesAsync();
    }

    public Task<Domain.Entities.Recibo?> GetByFaturaIdAsync(int faturaId)
        => _ctx.Recibos.AsNoTracking().FirstOrDefaultAsync(r => r.FaturaId == faturaId);

    public Task<Domain.Entities.Recibo?> GetByIdAsync(int id)
        => _ctx.Recibos.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
}
