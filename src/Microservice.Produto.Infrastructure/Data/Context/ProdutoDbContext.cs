using Microsoft.EntityFrameworkCore;
using Microservice.Produto.Domain.Entities;

namespace Microservice.Produto.Infrastructure.Data.Context;

public class ProdutoDbContext : DbContext
{
    public ProdutoDbContext(DbContextOptions<ProdutoDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos => Set<Produto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Nome).IsRequired();
            e.HasIndex(p => p.Nome).IsUnique();
            e.Property(p => p.Preco).HasPrecision(18, 2);
        });
    }
}
