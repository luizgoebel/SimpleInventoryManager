using Microservice.Recibo.Infrastructure.Data.Context;
using ReciboEntity = Microservice.Recibo.Domain.Entities.Recibo;

namespace Microservice.Recibo.API.Data.Seed;

public static class ReciboDbContextSeed
{
    public static async Task SeedAsync(this ReciboDbContext ctx)
    {
        if (ctx.Recibos.Any()) return;

        var r1 = new ReciboEntity(1, "RC-20240101-1-001", 240m);
        r1.Validar();
        var r2 = new ReciboEntity(2, "RC-20240101-2-002", 80m);
        r2.Validar();

        ctx.Recibos.AddRange(r1, r2);
        await ctx.SaveChangesAsync();
    }
}
