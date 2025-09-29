using Microservice.Pedido.Domain.Enums;
using Shared.Domain.Entities;
using Shared.Domain.Exceptions;

namespace Microservice.Pedido.Domain.Entities;

public class Pedido : BaseModel<Pedido>
{
    private readonly List<PedidoItem> _itens = new();
    public int Id { get; set; }
    public PedidoStatus Status { get; private set; } = PedidoStatus.Pendente;
    public IReadOnlyCollection<PedidoItem> Itens => _itens.AsReadOnly();
    public decimal Total { get; private set; }

    public void AdicionarItem(int produtoId, int quantidade, decimal precoUnitario)
    {
        PedidoItem item = new(produtoId, quantidade, precoUnitario);
        item.Validar();
        this._itens.Add(item);
        RecalcularTotal();
    }

    public void Confirmar()
    {
        if (Status != PedidoStatus.Pendente) throw new DomainException("Estado inválido para confirmação.");
        Status = PedidoStatus.Confirmado;
    }

    public void Cancelar()
    {
        if (Status == PedidoStatus.Cancelado) return;
        if (Status == PedidoStatus.Confirmado) throw new DomainException("Pedido confirmado não pode ser cancelado.");
        Status = PedidoStatus.Cancelado;
    }

    public void Validar()
    {
        if (!_itens.Any()) throw new DomainException("Pedido deve conter itens.");
        foreach (PedidoItem item in _itens) item.Validar();
        if (Total <= 0) throw new DomainException("Total inválido.");
    }

    private void RecalcularTotal() => Total = _itens.Sum(i => i.Subtotal);
}
