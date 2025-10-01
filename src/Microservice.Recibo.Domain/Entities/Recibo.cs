using Shared.Domain.Entities;
using Shared.Domain.Exceptions;

namespace Microservice.Recibo.Domain.Entities;

public class Recibo : BaseModel<Recibo>
{
    public int Id { get; set; }
    public string Numero { get; private set; } = string.Empty;
    public int FaturaId { get; private set; }
    public DateTime DataEmissao { get; private set; }
    public decimal ValorTotal { get; private set; }

    // Construtor para cria��o de dom�nio
    public Recibo(int faturaId, string numero, decimal valorTotal)
    {
        FaturaId = faturaId;
        Numero = numero;
        ValorTotal = valorTotal;
        DataEmissao = DateTime.UtcNow;
    }

    // Construtor sem par�metros para EF Core
    private Recibo() { }

    public void Validar()
    {
        if (FaturaId <= 0) throw new DomainException("Fatura inv�lida.");
        if (string.IsNullOrWhiteSpace(Numero)) throw new DomainException("N�mero inv�lido.");
        if (ValorTotal < 0) throw new DomainException("Valor total inv�lido.");
    }
}
