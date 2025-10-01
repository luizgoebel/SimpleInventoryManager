using Microservice.Faturamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Faturamento.Infrastructure.Data.Context;

public class FaturaDbContext : DbContext
{
    public FaturaDbContext(DbContextOptions<FaturaDbContext> options) : base(options) { }

    public DbSet<Fatura> Faturas => Set<Fatura>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fatura
        var fatura = modelBuilder.Entity<Fatura>();
        fatura.ToTable("Faturas");
        fatura.HasKey(x => x.Id);
        fatura.Property(x => x.Id).ValueGeneratedOnAdd();
        fatura.Property(x => x.Numero)
              .IsRequired()
              .HasMaxLength(50);
        fatura.HasIndex(x => x.Numero).IsUnique();
        fatura.Property(x => x.PedidoId).IsRequired();
        fatura.HasIndex(x => x.PedidoId); // consulta recorrente
        fatura.Property(x => x.DataEmissao).IsRequired();
        fatura.Property(x => x.Total).HasPrecision(18, 2);
        fatura.Property(x => x.Status).IsRequired();

        // Configura coleção como owned ligada à entidade raiz
        fatura.OwnsMany(f => f.Itens, itens =>
        {
            itens.ToTable("FaturaItens");
            itens.WithOwner().HasForeignKey("FaturaId");
            itens.HasKey("Id");
            itens.Property<int>("Id").ValueGeneratedOnAdd();
            itens.Property(i => i.ProdutoId).IsRequired();
            itens.Property(i => i.Quantidade).IsRequired();
            itens.Property(i => i.PrecoUnitario)
                 .HasPrecision(18, 2)
                 .IsRequired();
            // Subtotal é calculado (Quantidade * PrecoUnitario) e não deve ser mapeado como coluna
            itens.Ignore(i => i.Subtotal);
        });
    }
}