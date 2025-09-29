using Shared.Domain.Entities;
using Shared.Domain.Exceptions;

namespace Microservice.Pedido.Domain.Entities;

public class PedidoItem : BaseModel<PedidoItem>
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal => PrecoUnitario * Quantidade;

    public PedidoItem() { }

    public PedidoItem(int produtoId, int quantidade, decimal precoUnitario)
    {
        ProdutoId = produtoId;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
    }

    public void Validar()
    {
        if (ProdutoId <= 0) throw new DomainException("Produto inválido.");
        if (Quantidade <= 0) throw new DomainException("Quantidade deve ser maior que zero.");
        if (PrecoUnitario < 0) throw new DomainException("Preço unitário inválido.");
    }
}
