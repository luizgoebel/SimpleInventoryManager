namespace Microservice.Faturamento.Application.Interfaces;

public interface IReciboEmissaoClient
{
    Task<bool> EmitirReciboAsync(int faturaId, string numeroFatura, decimal valorTotal);
}
