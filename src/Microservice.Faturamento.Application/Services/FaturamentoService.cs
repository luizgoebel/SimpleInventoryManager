using AutoMapper;
using Microservice.Faturamento.Application.DTOs;
using Microservice.Faturamento.Application.Interfaces;
using Microservice.Faturamento.Application.Models;
using Microservice.Faturamento.Domain.Entities;
using Shared.Application.Exceptions;

namespace Microservice.Faturamento.Application.Services;

public class FaturamentoService : IFaturamentoService
{
    private readonly IFaturaRepository _repo;
    private readonly IPedidoConsultaClient _pedidoClient;
    private readonly IReciboEmissaoClient _reciboClient;
    private readonly IMapper _mapper;

    public FaturamentoService(
        IFaturaRepository repo,
        IPedidoConsultaClient pedidoClient,
        IReciboEmissaoClient reciboClient,
        IMapper mapper)
    {
        this._repo = repo;
        this._pedidoClient = pedidoClient;
        this._reciboClient = reciboClient;
        this._mapper = mapper;
    }

    public async Task<FaturaCriacaoResultadoDto> FaturarPedidoAsync(int pedidoId)
    {
        Fatura? pedidoJaFaturado = await this._repo.GetByPedidoIdAsync(pedidoId);
        if (pedidoJaFaturado != null) throw new ServiceException("Pedido já faturado.");
        PedidoResumo pedidoResumo = await this._pedidoClient.ObterPedidoAsync(pedidoId) ??
            throw new ServiceException("Pedido não encontrado.");
        if (!string.Equals(pedidoResumo.Status, "Confirmado", StringComparison.OrdinalIgnoreCase))
            throw new ServiceException("Pedido não confirmado.");
        string numero = GerarNumeroFatura(pedidoId);

        Fatura fatura = new(pedidoResumo.Id, numero);
        foreach (PedidoItemResumo resumoItemPedido in pedidoResumo.Itens)
            fatura.AdicionarItem(resumoItemPedido.ProdutoId, resumoItemPedido.Quantidade, resumoItemPedido.PrecoUnitario);

        fatura.Validar();
        await _repo.AddAsync(fatura);
        _ = this._reciboClient.EmitirReciboAsync(fatura.Id, fatura.Numero, fatura.Total);

       return this._mapper.Map<FaturaCriacaoResultadoDto>(fatura);
    }

    public async Task<FaturaDto?> ObterPorIdAsync(int id)
    {
        Fatura? fatura = await this._repo.GetByIdAsync(id) ??
            throw new ServiceException("Fatura não encontrada.");
        return this._mapper.Map<FaturaDto>(fatura);
    }

    public async Task<FaturaDto?> ObterPorPedidoAsync(int pedidoId)
    {
        Fatura? fatura = await this._repo.GetByPedidoIdAsync(pedidoId) ??
            throw new ServiceException("Fatura não encontrada.");
        return this._mapper.Map<FaturaDto>(fatura);
    }

    private string GerarNumeroFatura(int pedidoId)
        => $"FT-{DateTime.UtcNow:yyyyMMdd}-{pedidoId}-{Guid.NewGuid().ToString()[..6]}";
}
