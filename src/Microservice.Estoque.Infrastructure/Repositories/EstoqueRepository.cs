using Microservice.Estoque.Application.Interfaces;
using Microservice.Estoque.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Estoque.Infrastructure.Repositories;

public class EstoqueRepository : Repository<Domain.Entities.Estoque>, IEstoqueRepository
{
    public EstoqueRepository(EstoqueDbContext ctx) : base(ctx) { }

    public async Task<Domain.Entities.Estoque?> GetByProdutoIdAsync(int produtoId)
    {
        return await _ctx.Estoques.FirstOrDefaultAsync(e => e.ProdutoId == produtoId);
    }
}
