using Microservice.Faturamento.Application.DTOs;
using Microservice.Faturamento.Application.Interfaces;
using Microservice.Faturamento.Domain.Entities;
using Shared.Application.Exceptions;

namespace Microservice.Faturamento.Application.Services;

public class FaturamentoService : IFaturamentoService
{
    private readonly IFaturaRepository _repo;
    private readonly IPedidoConsultaClient _pedidoClient;
    private readonly IReciboEmissaoClient _reciboClient;

    public FaturamentoService(
        IFaturaRepository repo,
        IPedidoConsultaClient pedidoClient,
        IReciboEmissaoClient reciboClient)
    {
        _repo = repo;
        _pedidoClient = pedidoClient;
        _reciboClient = reciboClient;
    }

    public async Task<FaturaCriacaoResultadoDto> FaturarPedidoAsync(int pedidoId)
    {
        var existente = await _repo.GetByPedidoIdAsync(pedidoId);
        if (existente != null) throw new ServiceException("Pedido já faturado.");

        var pedido = await _pedidoClient.ObterPedidoAsync(pedidoId) ??
            throw new ServiceException("Pedido não encontrado.");

        if (!string.Equals(pedido.Status, "Confirmado", StringComparison.OrdinalIgnoreCase))
            throw new ServiceException("Pedido não confirmado.");

        string numero = GerarNumeroFatura(pedidoId);

        Fatura fatura = new(pedido.Id, numero);
        foreach (var item in pedido.Itens)
            fatura.AdicionarItem(item.ProdutoId, item.Quantidade, item.PrecoUnitario);

        fatura.Validar();

        await _repo.AddAsync(fatura);

        _ = _reciboClient.EmitirReciboAsync(fatura.Id, fatura.Numero, fatura.Total);

        return new FaturaCriacaoResultadoDto(fatura.Id, fatura.Numero, fatura.Total);
    }

    public async Task<FaturaDto?> ObterPorIdAsync(int id)
    {
        var fatura = await _repo.GetByIdAsync(id);
        return fatura == null ? null : Map(fatura);
    }

    public async Task<FaturaDto?> ObterPorPedidoAsync(int pedidoId)
    {
        var fatura = await _repo.GetByPedidoIdAsync(pedidoId);
        return fatura == null ? null : Map(fatura);
    }

    private FaturaDto Map(Fatura f)
        => new(
            f.Id,
            f.Numero,
            f.PedidoId,
            f.DataEmissao,
            f.Total,
            f.Itens.Select(i => new FaturaItemDto(i.ProdutoId, i.Quantidade, i.PrecoUnitario, i.Subtotal)),
            f.Status.ToString());

    private string GerarNumeroFatura(int pedidoId)
        => $"FT-{DateTime.UtcNow:yyyyMMdd}-{pedidoId}-{Guid.NewGuid().ToString()[..6]}";
}
