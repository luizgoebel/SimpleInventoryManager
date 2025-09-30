using Shared.Domain.Entities;
using Shared.Domain.Exceptions;

namespace Microservice.Faturamento.Domain.Entities;

public class FaturaItem : BaseModel<FaturaItem>
{
    public int Id { get; set; }
    public int ProdutoId { get; private set; }
    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public decimal Subtotal => Quantidade * PrecoUnitario;

    public FaturaItem(int produtoId, int quantidade, decimal precoUnitario)
    {
        ProdutoId = produtoId;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
    }

    public void Validar()
    {
        if (ProdutoId <= 0) throw new DomainException("Produto inválido.");
        if (Quantidade <= 0) throw new DomainException("Quantidade inválida.");
        if (PrecoUnitario < 0) throw new DomainException("Preço inválido.");
    }
}
