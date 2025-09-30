using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microservice.Recibo.Application.Interfaces;
using Microservice.Recibo.Domain.Entities;

namespace Microservice.Recibo.Infrastructure.Repositories;

public class ReciboRepository : IReciboRepository
{
    private static readonly List<Recibo> _db = new();

    public Task AddAsync(Recibo recibo)
    {
        recibo.Id = _db.Count + 1;
        _db.Add(recibo);
        return Task.CompletedTask;
    }

    public Task<Recibo?> GetByFaturaIdAsync(int faturaId)
        => Task.FromResult(_db.FirstOrDefault(r => r.FaturaId == faturaId));

    public Task<Recibo?> GetByIdAsync(int id)
        => Task.FromResult(_db.FirstOrDefault(r => r.Id == id));
}
