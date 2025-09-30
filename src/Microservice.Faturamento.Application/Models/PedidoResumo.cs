namespace Microservice.Faturamento.Application.Models;

public class PedidoResumo
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<PedidoItemResumo> Itens { get; set; } = new();
}

public class PedidoItemResumo
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}
