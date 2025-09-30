namespace Microservice.Faturamento.Application.DTOs;

public record FaturaItemDto(int ProdutoId, int Quantidade, decimal PrecoUnitario, decimal Subtotal);
public record FaturaDto(int Id, string Numero, int PedidoId, DateTime DataEmissao, decimal Total, IEnumerable<FaturaItemDto> Itens, string Status);
public record FaturaCriacaoResultadoDto(int FaturaId, string Numero, decimal Total);
