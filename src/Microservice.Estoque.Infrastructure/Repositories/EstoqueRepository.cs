using Microsoft.EntityFrameworkCore;
using Microservice.Estoque.Application.Interfaces;
using Microservice.Estoque.Domain.Entities;
using Microservice.Estoque.Infrastructure.Data.Context;

namespace Microservice.Estoque.Infrastructure.Repositories;

public class EstoqueRepository : Repository<Estoque>, IEstoqueRepository
{
    public EstoqueRepository(EstoqueDbContext ctx) : base(ctx) { }

    public async Task<Estoque?> GetByProdutoIdAsync(int produtoId)
    {
        return await _ctx.Estoques.FirstOrDefaultAsync(e => e.ProdutoId == produtoId);
    }
}
