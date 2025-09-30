using Microservice.Recibo.Domain.Entities;

namespace Microservice.Recibo.Application.Interfaces;

public interface IReciboRepository
{
    Task AddAsync(Recibo recibo);
    Task<Recibo?> GetByFaturaIdAsync(int faturaId);
    Task<Recibo?> GetByIdAsync(int id);
}
