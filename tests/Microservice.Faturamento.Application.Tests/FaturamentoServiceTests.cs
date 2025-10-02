using AutoMapper;
using Microservice.Faturamento.Application.Interfaces;
using Microservice.Faturamento.Application.Mapping;
using Microservice.Faturamento.Application.Models;
using Microservice.Faturamento.Application.Services;
using Microservice.Faturamento.Domain.Entities;
using Shared.Application.Exceptions;

namespace Microservice.Faturamento.Application.Tests;

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
        _repo = new Mock<IFaturaRepository>(MockBehavior.Strict);
        _pedidoClient = new Mock<IPedidoConsultaClient>(MockBehavior.Strict);
        _reciboClient = new Mock<IReciboEmissaoClient>(MockBehavior.Strict);
        var cfg = new MapperConfiguration(c => c.AddProfile<FaturamentoProfile>());
        _mapper = cfg.CreateMapper();
        _service = new FaturamentoService(_repo.Object, _pedidoClient.Object, _reciboClient.Object, _mapper);
    }

    [Test]
    public void FaturarPedidoAsync_PedidoJaFaturado_DeveLancarServiceException()
    {
        _repo.Setup(r => r.GetByPedidoIdAsync(1)).ReturnsAsync(new Fatura(1, "FT-1"));
        Assert.ThrowsAsync<ServiceException>(() => _service.FaturarPedidoAsync(1));
        _repo.Verify(r => r.GetByPedidoIdAsync(1), Times.Once);
    }

    [Test]
    public void FaturarPedidoAsync_PedidoInexistente_DeveLancarServiceException()
    {
        _repo.Setup(r => r.GetByPedidoIdAsync(2)).ReturnsAsync((Fatura?)null);
        _pedidoClient.Setup(c => c.ObterPedidoAsync(2)).ReturnsAsync((PedidoResumo?)null);
        Assert.ThrowsAsync<ServiceException>(() => _service.FaturarPedidoAsync(2));
        _repo.Verify(r => r.GetByPedidoIdAsync(2), Times.Once);
        _pedidoClient.Verify(c => c.ObterPedidoAsync(2), Times.Once);
    }

    [Test]
    public void FaturarPedidoAsync_PedidoNaoConfirmado_DeveLancarServiceException()
    {
        _repo.Setup(r => r.GetByPedidoIdAsync(3)).ReturnsAsync((Fatura?)null);
        _pedidoClient.Setup(c => c.ObterPedidoAsync(3)).ReturnsAsync(new PedidoResumo { Id = 3, Status = "Pendente" });
        Assert.ThrowsAsync<ServiceException>(() => _service.FaturarPedidoAsync(3));
        _repo.Verify(r => r.GetByPedidoIdAsync(3), Times.Once);
        _pedidoClient.Verify(c => c.ObterPedidoAsync(3), Times.Once);
    }

    [Test]
    public async Task FaturarPedidoAsync_PedidoValido_DeveGerarFaturaEChamarRecibo()
    {
        int pedidoId = 4;
        _repo.Setup(r => r.GetByPedidoIdAsync(pedidoId)).ReturnsAsync((Fatura?)null);
        _pedidoClient.Setup(c => c.ObterPedidoAsync(pedidoId)).ReturnsAsync(new PedidoResumo { Id = pedidoId, Status = "Confirmado", Itens = new() { new PedidoItemResumo { ProdutoId = 1, Quantidade = 2, PrecoUnitario = 10m } } });
        _repo.Setup(r => r.AddAsync(It.IsAny<Fatura>())).Returns(Task.CompletedTask);
        _reciboClient.Setup(r => r.EmitirReciboAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal>())).ReturnsAsync(true);

        var resp = await _service.FaturarPedidoAsync(pedidoId);

        Assert.That(resp, Is.Not.Null);
        Assert.That(resp.Total, Is.GreaterThan(0));
        _repo.Verify(r => r.AddAsync(It.Is<Fatura>(f => f.PedidoId == pedidoId)), Times.Once);
        _reciboClient.Verify(r => r.EmitirReciboAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal>()), Times.Once);
    }

    [Test]
    public void ObterPorIdAsync_Inexistente_DeveLancarServiceException()
    {
        _repo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync((Fatura?)null);
        Assert.ThrowsAsync<ServiceException>(() => _service.ObterPorIdAsync(10));
        _repo.Verify(r => r.GetByIdAsync(10), Times.Once);
    }

    [Test]
    public async Task ObterPorIdAsync_Existente_DeveRetornarFatura()
    {
        int id = 11;
        var f = new Fatura(id, "FT-1");
        f.AdicionarItem(1, 1, 10m);
        _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(f);
        var dto = await _service.ObterPorIdAsync(id);
        Assert.That(dto, Is.Not.Null);
        Assert.That(dto!.Total, Is.EqualTo(10m));
        _repo.Verify(r => r.GetByIdAsync(id), Times.Once);
    }
}
