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

    public Recibo(int faturaId, string numero, decimal valor)
    {
        FaturaId = faturaId;
        Numero = numero;
        ValorTotal = valor;
        DataEmissao = DateTime.UtcNow;
    }

    public void Validar()
    {
        if (FaturaId <= 0) throw new DomainException("Fatura inv�lida.");
        if (string.IsNullOrWhiteSpace(Numero)) throw new DomainException("N�mero inv�lido.");
        if (ValorTotal < 0) throw new DomainException("Valor inv�lido.");
    }
}
