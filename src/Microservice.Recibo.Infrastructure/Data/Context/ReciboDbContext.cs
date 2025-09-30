using Microsoft.EntityFrameworkCore;

namespace Microservice.Recibo.Infrastructure.Data.Context;

public class ReciboDbContext : DbContext
{
    public ReciboDbContext(DbContextOptions<ReciboDbContext> options) : base(options) { }

    public DbSet<Domain.Entities.Recibo> Recibos => Set<Domain.Entities.Recibo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Entities.Recibo>(e =>
        {
            e.ToTable("Recibos");
            e.HasKey(r => r.Id);
            e.Property(r => r.Numero)
                .IsRequired()
                .HasMaxLength(50);
            e.HasIndex(r => r.Numero).IsUnique();

            e.Property(r => r.FaturaId).IsRequired();
            e.Property(r => r.DataEmissao)
                .IsRequired()
                .HasColumnType("datetime2");
            e.Property(r => r.ValorTotal)
                .HasPrecision(18, 2);
        });
    }
}
