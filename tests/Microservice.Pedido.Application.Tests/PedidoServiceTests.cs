using AutoMapper;
using Microservice.Pedido.Application.DTOs;
using Microservice.Pedido.Application.Interfaces;
using Microservice.Pedido.Application.Mapping;
using Microservice.Pedido.Application.Services;
using Shared.Application.Exceptions;
using System.Collections.Generic;

namespace Microservice.Pedido.Application.Tests;

public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _repo = new();
    private readonly Mock<IEstoqueMovimentoClient> _estoqueClient = new();
    private readonly IMapper _mapper;
    private readonly PedidoService _service;

    public PedidoServiceTests()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<PedidoProfile>());
        _mapper = cfg.CreateMapper();
        _service = new PedidoService(_repo.Object, _mapper, _estoqueClient.Object);
    }

    [SetUp]
    public void SetUp()
    {
        // Garante isolamento: limpa histórico de chamadas entre testes
        _repo.Invocations.Clear();
        _estoqueClient.Invocations.Clear();
    }

    [Test]
    public void CriarAsync_SemItens_DeveLancarServiceException()
    {
        var dto = new PedidoCriacaoDto { Itens = new List<PedidoItemCriacaoDto>() };
        Assert.ThrowsAsync<ServiceException>(() => _service.CriarAsync(dto));
    }

    [Test]
    public void CriarAsync_ItemProdutoInvalido_DeveLancarServiceException()
    {
        var dto = new PedidoCriacaoDto { Itens = new() { new PedidoItemCriacaoDto { ProdutoId = 0, Quantidade = 1, PrecoUnitario = 10 } } };
        Assert.ThrowsAsync<ServiceException>(() => _service.CriarAsync(dto));
    }

    [Test]
    public void CriarAsync_ItemQuantidadeInvalida_DeveLancarServiceException()
    {
        var dto = new PedidoCriacaoDto { Itens = new() { new PedidoItemCriacaoDto { ProdutoId = 1, Quantidade = 0, PrecoUnitario = 10 } } };
        Assert.ThrowsAsync<ServiceException>(() => _service.CriarAsync(dto));
    }

    [Test]
    public void CriarAsync_ItemPrecoInvalido_DeveLancarServiceException()
    {
        var dto = new PedidoCriacaoDto { Itens = new() { new PedidoItemCriacaoDto { ProdutoId = 1, Quantidade = 2, PrecoUnitario = -1 } } };
        Assert.ThrowsAsync<ServiceException>(() => _service.CriarAsync(dto));
    }

    [Test]
    public async Task CriarAsync_ItensValidos_DeveCriarPedidoEChamarEstoque()
    {
        var dto = new PedidoCriacaoDto { Itens = new() { new PedidoItemCriacaoDto { ProdutoId = 1, Quantidade = 2, PrecoUnitario = 10m }, new PedidoItemCriacaoDto { ProdutoId = 2, Quantidade = 1, PrecoUnitario = 5m } } };
        var resp = await _service.CriarAsync(dto);
        Assert.That(resp.Itens.Count, Is.EqualTo(2));
        _repo.Verify(r => r.AddAsync(It.IsAny<Microservice.Pedido.Domain.Entities.Pedido>()), Times.Once);
        _estoqueClient.Verify(c => c.RegistrarSaidaAsync(1, 2), Times.Once);
        _estoqueClient.Verify(c => c.RegistrarSaidaAsync(2, 1), Times.Once);
    }

    [Test]
    public void GetByIdAsync_Inexistente_DeveLancarServiceException()
    {
        _repo.Setup(r => r.GetByIdWithItemsAsync(1)).ReturnsAsync((Microservice.Pedido.Domain.Entities.Pedido?)null);
        Assert.ThrowsAsync<ServiceException>(() => _service.GetByIdAsync(1));
    }

    [Test]
    public async Task GetByIdAsync_Existente_DeveRetornarDto()
    {
        var pedido = new Microservice.Pedido.Domain.Entities.Pedido();
        pedido.AdicionarItem(1, 1, 10m);
        _repo.Setup(r => r.GetByIdWithItemsAsync(1)).ReturnsAsync(pedido);
        var dto = await _service.GetByIdAsync(1);
        Assert.That(dto, Is.Not.Null);
        Assert.That(dto!.Itens.Count, Is.EqualTo(1));
    }

    [Test]
    public void CancelarAsync_Inexistente_DeveLancarServiceException()
    {
        _repo.Setup(r => r.GetByIdWithItemsAsync(1)).ReturnsAsync((Microservice.Pedido.Domain.Entities.Pedido?)null);
        Assert.ThrowsAsync<ServiceException>(() => _service.CancelarAsync(1));
    }

    [Test]
    public async Task CancelarAsync_Pendente_DeveCancelar()
    {
        var pedido = new Microservice.Pedido.Domain.Entities.Pedido();
        pedido.AdicionarItem(1, 1, 10m);
        _repo.Setup(r => r.GetByIdWithItemsAsync(1)).ReturnsAsync(pedido);
        var ok = await _service.CancelarAsync(1);
        Assert.That(ok, Is.True);
        _repo.Verify(r => r.UpdateAsync(It.Is<Microservice.Pedido.Domain.Entities.Pedido>(p => p.Status.ToString() == "Cancelado")), Times.Once);
    }

    [Test]
    public async Task CancelarAsync_JaCancelado_NaoAltera()
    {
        var pedido = new Microservice.Pedido.Domain.Entities.Pedido();
        pedido.AdicionarItem(1, 1, 10m);
        pedido.Cancelar();
        _repo.Setup(r => r.GetByIdWithItemsAsync(1)).ReturnsAsync(pedido);
        var ok = await _service.CancelarAsync(1);
        Assert.That(ok, Is.True);
        _repo.Verify(r => r.UpdateAsync(It.IsAny<Microservice.Pedido.Domain.Entities.Pedido>()), Times.Once);
    }
}
