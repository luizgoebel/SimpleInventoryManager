namespace Microservice.Recibo.Application.DTOs;

public class EmissaoReciboDto
{
    public int FaturaId { get; set; }
    public string NumeroFatura { get; set; }
    public decimal ValorTotal { get; set; }
    
}
