using Microsoft.EntityFrameworkCore;

namespace Microservice.Produto.Infrastructure.Data.Context;

public class ProdutoDbContext : DbContext
{
    public ProdutoDbContext(DbContextOptions<ProdutoDbContext> options) : base(options) { }

    public DbSet<Microservice.Produto.Domain.Entities.Produto> Produtos => Set<Microservice.Produto.Domain.Entities.Produto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Microservice.Produto.Domain.Entities.Produto>(e =>
        {
            e.ToTable("Produtos");
            e.HasKey(p => p.Id);
            e.Property(p => p.Nome).IsRequired();
            e.HasIndex(p => p.Nome).IsUnique();
            e.Property(p => p.Preco).HasPrecision(18, 2);
        });
    }
}
