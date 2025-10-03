using AutoMapper;
using Microservice.Estoque.Application.DTOs;
using Microservice.Estoque.Application.Interfaces;
using Microservice.Estoque.Application.Mapping;
using Microservice.Estoque.Application.Services;
using Shared.Application.Exceptions;

namespace Microservice.Estoque.Application.Tests.EstoqueServiceTests;

public class EstoqueServiceTests
{
    private readonly Mock<IEstoqueRepository> _repo = new();
    private readonly IMapper _mapper;
    private readonly EstoqueService _service;

    public EstoqueServiceTests()
    {
        MapperConfiguration cfg = new MapperConfiguration(c => c.AddProfile<EstoqueProfile>());
        this._mapper = cfg.CreateMapper();
        this._service = new EstoqueService(this._repo.Object, this._mapper);
    }

    [Test]
    public async Task CriarInicialSeNaoExisteAsync_NaoExiste_DeveCriar()
    {
        this._repo.Setup(r => r.GetByProdutoIdAsync(1)).ReturnsAsync((Domain.Entities.Estoque?)null);
        bool ok = await this._service.CriarInicialSeNaoExisteAsync(1);
        Assert.That(ok, Is.True);
        this._repo.Verify(r => r.AddAsync(It.Is<Domain.Entities.Estoque>(e => e.ProdutoId == 1)), Times.Once);
    }

    [Test]
    public async Task CriarInicialSeNaoExisteAsync_JaExiste_DeveNaoDuplicar()
    {
        this._repo.Setup(r => r.GetByProdutoIdAsync(1)).ReturnsAsync(new Domain.Entities.Estoque { ProdutoId = 1, Quantidade = 5 });
        await this._service.CriarInicialSeNaoExisteAsync(1);
        this._repo.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Estoque>()), Times.Never);
    }

    [Test]
    public async Task EntradaAsync_EstoqueInexistente_DeveCriarEAdicionar()
    {
        this._repo.Setup(r => r.GetByProdutoIdAsync(1)).ReturnsAsync((Domain.Entities.Estoque?)null);
        MovimentoEstoqueDto dto = new MovimentoEstoqueDto { ProdutoId = 1, Quantidade = 3 };
        bool ok = await this._service.EntradaAsync(dto);
        Assert.That(ok, Is.True);
        this._repo.Verify(r => r.AddAsync(It.Is<Domain.Entities.Estoque>(e => e.Quantidade == 0)), Times.Once);
        this._repo.Verify(r => r.UpdateAsync(It.Is<Domain.Entities.Estoque>(e => e.Quantidade == 3)), Times.Once);
    }

    [Test]
    public void EntradaAsync_QuantidadeNegativa_DeveLancarServiceException()
    {
        MovimentoEstoqueDto dto = new MovimentoEstoqueDto { ProdutoId = 1, Quantidade = -1 };
        Assert.ThrowsAsync<ServiceException>(() => this._service.EntradaAsync(dto));
    }

    [Test]
    public void SaidaAsync_EstoqueInexistente_DeveLancarServiceException()
    {
        MovimentoEstoqueDto dto = new MovimentoEstoqueDto { ProdutoId = 1, Quantidade = 1 };
        Assert.ThrowsAsync<ServiceException>(() => this._service.SaidaAsync(dto));
    }

    [Test]
    public void SaidaAsync_QuantidadeMaiorQueSaldo_DeveLancarServiceException()
    {
        this._repo.Setup(r => r.GetByProdutoIdAsync(1)).ReturnsAsync(new Domain.Entities.Estoque { ProdutoId = 1, Quantidade = 2 });
        MovimentoEstoqueDto dto = new MovimentoEstoqueDto { ProdutoId = 1, Quantidade = 5 };
        Assert.ThrowsAsync<ServiceException>(() => this._service.SaidaAsync(dto));
    }

    [Test]
    public async Task SaidaAsync_QuantidadeValida_DeveAtualizar()
    {
        Domain.Entities.Estoque estoque = new Domain.Entities.Estoque { ProdutoId = 1, Quantidade = 5 };
        this._repo.Setup(r => r.GetByProdutoIdAsync(1)).ReturnsAsync(estoque);
        MovimentoEstoqueDto dto = new MovimentoEstoqueDto { ProdutoId = 1, Quantidade = 3 };
        bool ok = await this._service.SaidaAsync(dto);
        Assert.That(ok, Is.True);
        this._repo.Verify(r => r.UpdateAsync(It.Is<Domain.Entities.Estoque>(e => e.Quantidade == 2)), Times.Once);
    }
}
