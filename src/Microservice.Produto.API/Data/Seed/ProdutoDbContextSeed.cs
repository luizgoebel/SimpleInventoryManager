using Microservice.Produto.Infrastructure.Data.Context;
using ProdutoEntity = Microservice.Produto.Domain.Entities.Produto;

namespace Microservice.Produto.API.Data.Seed;

public static class ProdutoDbContextSeed
{
    public static async Task SeedAsync(this ProdutoDbContext ctx)
    {
        if (ctx.Produtos.Any()) return;

        var p1 = new ProdutoEntity { Nome = "Teclado", Preco = 120m, Descricao = "Teclado mecânico" };
        p1.Validar();
        var p2 = new ProdutoEntity { Nome = "Mouse", Preco = 80m, Descricao = "Mouse óptico" };
        p2.Validar();

        ctx.Produtos.AddRange(p1, p2);
        await ctx.SaveChangesAsync();
    }
}
