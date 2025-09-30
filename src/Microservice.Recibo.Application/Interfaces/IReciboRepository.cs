namespace Microservice.Recibo.Application.Interfaces;

public interface IReciboRepository
{
    Task AddAsync(Domain.Entities.Recibo recibo);
    Task<Domain.Entities.Recibo?> GetByFaturaIdAsync(int faturaId);
    Task<Domain.Entities.Recibo?> GetByIdAsync(int id);
}
