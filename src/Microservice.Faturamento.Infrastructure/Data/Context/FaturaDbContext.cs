using Microservice.Faturamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Faturamento.Infrastructure.Data.Context;

public class FaturaDbContext : DbContext
{
    public FaturaDbContext(DbContextOptions<FaturaDbContext> options) : base(options) { }

    public DbSet<Fatura> Faturas => Set<Fatura>();
    public DbSet<FaturaItem> FaturaItens => Set<FaturaItem>();

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
        fatura.Property(x => x.PedidoId).IsRequired();
        fatura.Property(x => x.DataEmissao).IsRequired();
        fatura.Property(x => x.Total).HasColumnType("decimal(18,2)");
        fatura.Property(x => x.Status).IsRequired();

        // Mapeamento da coleção de itens usando backing field (_itens)
        fatura.Metadata.FindNavigation(nameof(Fatura.Itens))!
              .SetPropertyAccessMode(PropertyAccessMode.Field);

        fatura.OwnsMany<FaturaItem>("_itens", itens =>
        {
            itens.ToTable("FaturaItens");
            itens.WithOwner().HasForeignKey("FaturaId");
            itens.HasKey("Id");
            itens.Property<int>("Id").ValueGeneratedOnAdd();
            itens.Property(i => i.ProdutoId).IsRequired();
            itens.Property(i => i.Quantidade).IsRequired();
            itens.Property(i => i.PrecoUnitario)
                 .HasColumnType("decimal(18,2)")
                 .IsRequired();
            itens.Property(i => i.Subtotal)
                 .HasColumnType("decimal(18,2)")
                 .IsRequired();
        });
    }
}