using AutoMapper;
using Microservice.Recibo.Application.DTOs;
using Microservice.Recibo.Application.Interfaces;
using Shared.Application.Exceptions;

namespace Microservice.Recibo.Application.Services;

public class ReciboService : IReciboService
{
    private readonly IReciboRepository _repo;
    private readonly IMapper _mapper;

    public ReciboService(IReciboRepository repo, IMapper mapper)
    {
        this._repo = repo;
        this._mapper = mapper;
    }

    public async Task<ReciboDto> GerarPorFaturaAsync(int faturaId, string numeroFatura, decimal valorTotal)
    {
        Domain.Entities.Recibo? reciboExiste = await this._repo.GetByFaturaIdAsync(faturaId);
        if (reciboExiste != null) throw new ServiceException("Recibo existente.");
        string numeroRecibo = GerarNumeroRecibo(faturaId, numeroFatura);
        Domain.Entities.Recibo recibo = new(faturaId, numeroRecibo, valorTotal);
        recibo.Validar();
        await this._repo.AddAsync(recibo);
        return _mapper.Map<ReciboDto>(recibo);
    }

    public async Task<ReciboDto?> ObterPorFaturaAsync(int faturaId)
    {
        Domain.Entities.Recibo? recibo = await this._repo.GetByFaturaIdAsync(faturaId) ??
            throw new ServiceException("Recibo não gerado.");
        return _mapper.Map<ReciboDto>(recibo);
    }

    public async Task<ReciboDto?> ObterPorIdAsync(int id)
    {
        Domain.Entities.Recibo? recibo = await this._repo.GetByIdAsync(id) ??
            throw new ServiceException("Recibo não gerado.");
        return _mapper.Map<ReciboDto>(recibo);
    }

    private string GerarNumeroRecibo(int faturaId, string numeroFatura)
        => $"RC-{DateTime.UtcNow:yyyyMMdd}-{faturaId}-{numeroFatura.Split('-').Last()}";
}
