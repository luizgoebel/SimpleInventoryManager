using Microservice.Faturamento.Domain.Enums;
using Shared.Domain.Entities;
using Shared.Domain.Exceptions;

namespace Microservice.Faturamento.Domain.Entities;

public class Fatura : BaseModel<Fatura>
{
    private readonly List<FaturaItem> _itens = new();

    public int Id { get; set; }
    public string Numero { get; private set; } = string.Empty;
    public int PedidoId { get; private set; }
    public DateTime DataEmissao { get; private set; }
    public decimal Total { get; private set; }
    public FaturaStatus Status { get; private set; } = FaturaStatus.Emitida;
    public IReadOnlyCollection<FaturaItem> Itens => _itens.AsReadOnly();

    public Fatura(int pedidoId, string numero)
    {
        PedidoId = pedidoId;
        Numero = numero;
        DataEmissao = DateTime.UtcNow;
    }

    public void AdicionarItem(int produtoId, int quantidade, decimal precoUnitario)
    {
        FaturaItem item = new(produtoId, quantidade, precoUnitario);
        item.Validar();
        _itens.Add(item);
        RecalcularTotal();
    }

    public void Cancelar()
    {
        if (Status == FaturaStatus.Cancelada) return;
        Status = FaturaStatus.Cancelada;
    }

    public void Validar()
    {
        if (PedidoId <= 0) throw new DomainException("PedidoId inválido.");
        if (string.IsNullOrWhiteSpace(Numero)) throw new DomainException("Número inválido.");
        if (!_itens.Any()) throw new DomainException("Fatura deve conter itens.");
        if (Total <= 0) throw new DomainException("Total inválido.");
    }

    private void RecalcularTotal() => Total = _itens.Sum(i => i.Subtotal);
}
