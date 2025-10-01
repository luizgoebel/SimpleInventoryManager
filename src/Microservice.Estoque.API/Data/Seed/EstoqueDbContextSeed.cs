using Microservice.Estoque.Infrastructure.Data.Context;
using EstoqueEntity = Microservice.Estoque.Domain.Entities.Estoque;

namespace Microservice.Estoque.API.Data.Seed;

public static class EstoqueDbContextSeed
{
    public static async Task SeedAsync(this EstoqueDbContext ctx)
    {
        if (ctx.Estoques.Any()) return;

        ctx.Estoques.AddRange(
            new EstoqueEntity { ProdutoId = 1, Quantidade = 50 },
            new EstoqueEntity { ProdutoId = 2, Quantidade = 30 }
        );
        await ctx.SaveChangesAsync();
    }
}
