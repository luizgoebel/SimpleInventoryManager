using AutoMapper;
using Microservice.Faturamento.Application.DTOs;
using Microservice.Faturamento.Application.Interfaces;
using Microservice.Faturamento.Application.Mapping;
using Microservice.Faturamento.Application.Models;
using Microservice.Faturamento.Application.Services;
using Microservice.Faturamento.Domain.Entities;
using Shared.Application.Exceptions;
using System.Collections.Generic;

namespace Microservice.Faturamento.Application.Tests.FaturamentoServiceTests;

public class FaturamentoServiceTests
{
    private Mock<IFaturaRepository> _repo = null!;
    private Mock<IPedidoConsultaClient> _pedidoClient = null!;
    private Mock<IReciboEmissaoClient> _reciboClient = null!;
    private IMapper _mapper = null!;
    private FaturamentoService _service = null!;

    [SetUp]
    public void SetUp()
    {
        this._repo = new Mock<IFaturaRepository>(MockBehavior.Strict);
        this._pedidoClient = new Mock<IPedidoConsultaClient>(MockBehavior.Strict);
        this._reciboClient = new Mock<IReciboEmissaoClient>(MockBehavior.Strict);
        MapperConfiguration cfg = new MapperConfiguration(c => c.AddProfile<FaturamentoProfile>());
        this._mapper = cfg.CreateMapper();
        this._service = new FaturamentoService(this._repo.Object, this._pedidoClient.Object, this._reciboClient.Object, this._mapper);
    }

    [Test]
    public void FaturarPedidoAsync_PedidoJaFaturado_DeveLancarServiceException()
    {
        this._repo.Setup(r => r.GetByPedidoIdAsync(1)).ReturnsAsync(new Fatura(1, "FT-1"));
        Assert.ThrowsAsync<ServiceException>(() => this._service.FaturarPedidoAsync(1));
        this._repo.Verify(r => r.GetByPedidoIdAsync(1), Times.Once);
    }

    [Test]
    public void FaturarPedidoAsync_PedidoInexistente_DeveLancarServiceException()
    {
        this._repo.Setup(r => r.GetByPedidoIdAsync(2)).ReturnsAsync((Fatura?)null);
        this._pedidoClient.Setup(c => c.ObterPedidoAsync(2)).ReturnsAsync((PedidoResumo?)null);
        Assert.ThrowsAsync<ServiceException>(() => this._service.FaturarPedidoAsync(2));
        this._repo.Verify(r => r.GetByPedidoIdAsync(2), Times.Once);
        this._pedidoClient.Verify(c => c.ObterPedidoAsync(2), Times.Once);
    }

    [Test]
    public void FaturarPedidoAsync_PedidoNaoConfirmado_DeveLancarServiceException()
    {
        this._repo.Setup(r => r.GetByPedidoIdAsync(3)).ReturnsAsync((Fatura?)null);
        this._pedidoClient.Setup(c => c.ObterPedidoAsync(3)).ReturnsAsync(new PedidoResumo { Id = 3, Status = "Pendente" });
        Assert.ThrowsAsync<ServiceException>(() => this._service.FaturarPedidoAsync(3));
        this._repo.Verify(r => r.GetByPedidoIdAsync(3), Times.Once);
        this._pedidoClient.Verify(c => c.ObterPedidoAsync(3), Times.Once);
    }

    [Test]
    public async Task FaturarPedidoAsync_PedidoValido_DeveGerarFaturaEChamarRecibo()
    {
        int pedidoId = 4;
        this._repo.Setup(r => r.GetByPedidoIdAsync(pedidoId)).ReturnsAsync((Fatura?)null);
        this._pedidoClient.Setup(c => c.ObterPedidoAsync(pedidoId)).ReturnsAsync(new PedidoResumo
        {
            Id = pedidoId,
            Status = "Confirmado",
            Itens = new List<PedidoItemResumo>
            {
                new PedidoItemResumo { ProdutoId = 1, Quantidade = 2, PrecoUnitario = 10m }
            }
        });
        this._repo.Setup(r => r.AddAsync(It.IsAny<Fatura>())).Returns(Task.CompletedTask);
        this._reciboClient.Setup(r => r.EmitirReciboAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal>())).ReturnsAsync(true);

        FaturaCriacaoResultadoDto resp = await this._service.FaturarPedidoAsync(pedidoId);

        Assert.That(resp, Is.Not.Null);
        Assert.That(resp.Total, Is.GreaterThan(0));
        this._repo.Verify(r => r.AddAsync(It.Is<Fatura>(f => f.PedidoId == pedidoId)), Times.Once);
        this._reciboClient.Verify(r => r.EmitirReciboAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal>()), Times.Once);
    }

    [Test]
    public void ObterPorIdAsync_Inexistente_DeveLancarServiceException()
    {
        this._repo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync((Fatura?)null);
        Assert.ThrowsAsync<ServiceException>(() => this._service.ObterPorIdAsync(10));
        this._repo.Verify(r => r.GetByIdAsync(10), Times.Once);
    }

    [Test]
    public async Task ObterPorIdAsync_Existente_DeveRetornarFatura()
    {
        int id = 11;
        Fatura f = new Fatura(id, "FT-1");
        f.AdicionarItem(1, 1, 10m);
        this._repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(f);
        FaturaDto? dto = await this._service.ObterPorIdAsync(id);
        Assert.That(dto, Is.Not.Null);
        Assert.That(dto!.Total, Is.EqualTo(10m));
        this._repo.Verify(r => r.GetByIdAsync(id), Times.Once);
    }
}
