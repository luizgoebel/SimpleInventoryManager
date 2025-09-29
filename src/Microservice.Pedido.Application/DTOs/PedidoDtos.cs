namespace Microservice.Pedido.Application.DTOs;

public class PedidoItemCriacaoDto
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}

public class PedidoCriacaoDto
{
    public List<PedidoItemCriacaoDto> Itens { get; set; } = new();
}

public class PedidoItemDto
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal { get; set; }
}

public class PedidoDto
{
    public int Id { get; set; }
    public DateTime DataCriacao { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<PedidoItemDto> Itens { get; set; } = new();
}
