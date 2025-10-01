using Microsoft.EntityFrameworkCore;

namespace Microservice.Faturamento.Infrastructure.Data.Context;

public class FaturaDbContext : DbContext
{
    public FaturaDbContext(DbContextOptions<FaturaDbContext> options) : base(options) { }
    public DbSet<Domain.Entities.Fatura> Faturas => Set<Domain.Entities.Fatura>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Entities.Fatura>(e =>
        {
            e.ToTable("Faturas");
            e.HasKey(f => f.Id);
            e.Property(f => f.Numero)
                .IsRequired()
                .HasMaxLength(50);
            e.HasIndex(f => f.Numero).IsUnique();
            e.Property(f => f.PedidoId).IsRequired();
            e.Property(f => f.DataEmissao)
                .IsRequired()
                .HasColumnType("datetime2");
            e.Property(f => f.Total)
                .HasPrecision(18, 2);
        });
    }
}
