using Microservice.Faturamento.Domain.Entities;
using Microservice.Faturamento.Infrastructure.Data.Context;

namespace Microservice.Faturamento.API.Data.Seed;

public static class FaturaDbContextSeed
{
    public static async Task SeedAsync(this FaturaDbContext ctx)
    {
        if (ctx.Faturas.Any()) return;

        var f1 = new Fatura(1, "FAT-202401-001");
        f1.AdicionarItem(1, 2, 120m);
        f1.Validar();
        var f2 = new Fatura(2, "FAT-202401-002");
        f2.AdicionarItem(2, 1, 80m);
        f2.Validar();

        ctx.Faturas.AddRange(f1, f2);
        await ctx.SaveChangesAsync();
    }
}
