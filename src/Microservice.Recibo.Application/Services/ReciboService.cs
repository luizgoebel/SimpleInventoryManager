using Microservice.Recibo.Application.DTOs;
using Microservice.Recibo.Application.Interfaces;
using Microservice.Recibo.Domain.Entities;
using Shared.Application.Exceptions;

namespace Microservice.Recibo.Application.Services;

public class ReciboService : IReciboService
{
    private readonly IReciboRepository _repo;

    public ReciboService(IReciboRepository repo) => _repo = repo;

    public async Task<ReciboDto> GerarPorFaturaAsync(int faturaId, string numeroFatura, decimal valorTotal)
    {
        var existente = await _repo.GetByFaturaIdAsync(faturaId);
        if (existente != null) throw new ServiceException("Recibo já existe.");

        string numeroRecibo = GerarNumeroRecibo(faturaId, numeroFatura);
        Recibo recibo = new(faturaId, numeroRecibo, valorTotal);
        recibo.Validar();
        await _repo.AddAsync(recibo);
        return Map(recibo);
    }

    public async Task<ReciboDto?> ObterPorFaturaAsync(int faturaId)
    {
        var recibo = await _repo.GetByFaturaIdAsync(faturaId);
        return recibo == null ? null : Map(recibo);
    }

    public async Task<ReciboDto?> ObterPorIdAsync(int id)
    {
        var recibo = await _repo.GetByIdAsync(id);
        return recibo == null ? null : Map(recibo);
    }

    private ReciboDto Map(Recibo r) => new(r.Id, r.Numero, r.FaturaId, r.DataEmissao, r.ValorTotal);

    private string GerarNumeroRecibo(int faturaId, string numeroFatura)
        => $"RC-{DateTime.UtcNow:yyyyMMdd}-{faturaId}-{numeroFatura.Split('-').Last()}";
}
