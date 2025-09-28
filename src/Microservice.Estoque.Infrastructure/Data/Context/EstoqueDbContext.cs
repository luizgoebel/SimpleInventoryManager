using Microsoft.EntityFrameworkCore;
using Microservice.Estoque.Domain.Entities;

namespace Microservice.Estoque.Infrastructure.Data.Context;

public class EstoqueDbContext : DbContext
{
    public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options) { }

    public DbSet<Microservice.Estoque.Domain.Entities.Estoque> Estoques => Set<Microservice.Estoque.Domain.Entities.Estoque>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Microservice.Estoque.Domain.Entities.Estoque>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.ProdutoId).IsUnique();
            e.Property(x => x.Quantidade).IsRequired();
        });
    }
}
