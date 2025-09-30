namespace Microservice.Recibo.Application.DTOs;

public record ReciboDto(int Id, string Numero, int FaturaId, DateTime DataEmissao, decimal ValorTotal);
