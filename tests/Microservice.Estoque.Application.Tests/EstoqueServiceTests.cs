using AutoMapper;
using Microservice.Estoque.Application.DTOs;
using Microservice.Estoque.Application.Interfaces;
using Microservice.Estoque.Application.Mapping;
using Microservice.Estoque.Application.Services;
using Shared.Application.Exceptions;

namespace Microservice.Estoque.Application.Tests;

public class EstoqueServiceTests
{
    private readonly Mock<IEstoqueRepository> _repo = new();
    private readonly IMapper _mapper;
    private readonly EstoqueService _service;

    public EstoqueServiceTests()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<EstoqueProfile>());
        _mapper = cfg.CreateMapper();
        _service = new EstoqueService(_repo.Object, _mapper);
    }

    [Test]
    public async Task CriarInicialSeNaoExisteAsync_NaoExiste_DeveCriar()
    {
        _repo.Setup(r => r.GetByProdutoIdAsync(1)).ReturnsAsync((Microservice.Estoque.Domain.Entities.Estoque?)null);
        var ok = await _service.CriarInicialSeNaoExisteAsync(1);
        Assert.That(ok, Is.True);
        _repo.Verify(r => r.AddAsync(It.Is<Microservice.Estoque.Domain.Entities.Estoque>(e => e.ProdutoId == 1)), Times.Once);
    }

    [Test]
    public async Task CriarInicialSeNaoExisteAsync_JaExiste_DeveNaoDuplicar()
    {
        _repo.Setup(r => r.GetByProdutoIdAsync(1)).ReturnsAsync(new Microservice.Estoque.Domain.Entities.Estoque { ProdutoId = 1, Quantidade = 5 });
        await _service.CriarInicialSeNaoExisteAsync(1);
        _repo.Verify(r => r.AddAsync(It.IsAny<Microservice.Estoque.Domain.Entities.Estoque>()), Times.Never);
    }

    [Test]
    public async Task EntradaAsync_EstoqueInexistente_DeveCriarEAdicionar()
    {
        _repo.Setup(r => r.GetByProdutoIdAsync(1)).ReturnsAsync((Microservice.Estoque.Domain.Entities.Estoque?)null);
        var dto = new MovimentoEstoqueDto { ProdutoId = 1, Quantidade = 3 };
        var ok = await _service.EntradaAsync(dto);
        Assert.That(ok, Is.True);
        _repo.Verify(r => r.AddAsync(It.Is<Microservice.Estoque.Domain.Entities.Estoque>(e => e.Quantidade == 0)), Times.Once);
        _repo.Verify(r => r.UpdateAsync(It.Is<Microservice.Estoque.Domain.Entities.Estoque>(e => e.Quantidade == 3)), Times.Once);
    }

    [Test]
    public void EntradaAsync_QuantidadeNegativa_DeveLancarServiceException()
    {
        var dto = new MovimentoEstoqueDto { ProdutoId = 1, Quantidade = -1 };
        Assert.ThrowsAsync<ServiceException>(() => _service.EntradaAsync(dto));
    }

    [Test]
    public void SaidaAsync_EstoqueInexistente_DeveLancarServiceException()
    {
        var dto = new MovimentoEstoqueDto { ProdutoId = 1, Quantidade = 1 };
        Assert.ThrowsAsync<ServiceException>(() => _service.SaidaAsync(dto));
    }

    [Test]
    public void SaidaAsync_QuantidadeMaiorQueSaldo_DeveLancarServiceException()
    {
        _repo.Setup(r => r.GetByProdutoIdAsync(1)).ReturnsAsync(new Microservice.Estoque.Domain.Entities.Estoque { ProdutoId = 1, Quantidade = 2 });
        var dto = new MovimentoEstoqueDto { ProdutoId = 1, Quantidade = 5 };
        Assert.ThrowsAsync<ServiceException>(() => _service.SaidaAsync(dto));
    }

    [Test]
    public async Task SaidaAsync_QuantidadeValida_DeveAtualizar()
    {
        var estoque = new Microservice.Estoque.Domain.Entities.Estoque { ProdutoId = 1, Quantidade = 5 };
        _repo.Setup(r => r.GetByProdutoIdAsync(1)).ReturnsAsync(estoque);
        var dto = new MovimentoEstoqueDto { ProdutoId = 1, Quantidade = 3 };
        var ok = await _service.SaidaAsync(dto);
        Assert.That(ok, Is.True);
        _repo.Verify(r => r.UpdateAsync(It.Is<Microservice.Estoque.Domain.Entities.Estoque>(e => e.Quantidade == 2)), Times.Once);
    }
}
