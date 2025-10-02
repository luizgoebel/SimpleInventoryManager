using AutoMapper;
using Microservice.Produto.Application.DTOs;
using Microservice.Produto.Application.Interfaces;
using Microservice.Produto.Application.Mapping;
using Microservice.Produto.Application.Services;
using Shared.Application.Exceptions;
using Shared.Domain.Exceptions;

namespace Microservice.Produto.Application.Tests;

public class ProdutoServiceTests
{
    private Mock<IProdutoRepository> _repo = null!;
    private Mock<IEstoqueClient> _estoqueClient = null!;
    private IMapper _mapper = null!;
    private ProdutoService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IProdutoRepository>(MockBehavior.Strict);
        _estoqueClient = new Mock<IEstoqueClient>(MockBehavior.Loose);
        var cfg = new MapperConfiguration(c => c.AddProfile<ProdutoProfile>());
        _mapper = cfg.CreateMapper();
        _service = new ProdutoService(_repo.Object, _mapper, _estoqueClient.Object);
    }

    [Test]
    public async Task CriarProdutoAsync_DadosValidos_DeveCriarProduto()
    {
        var dto = new ProdutoCriacaoDto { Nome = "Teclado", Preco = 100m, Descricao = "Desc" };

        _repo.Setup(r => r.AddAsync(It.IsAny<Microservice.Produto.Domain.Entities.Produto>())).Returns(Task.CompletedTask);
        _estoqueClient.Setup(e => e.CriarEstoqueInicialAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

        var result = await _service.CriarProdutoAsync(dto);

        _repo.Verify(r => r.AddAsync(It.IsAny<Microservice.Produto.Domain.Entities.Produto>()), Times.Once);
        _estoqueClient.Verify(e => e.CriarEstoqueInicialAsync(It.IsAny<int>()), Times.Once);
        Assert.That(result.Nome, Is.EqualTo(dto.Nome));
    }

    [Test]
    public void CriarProdutoAsync_PrecoInvalido_DeveLancarDomainException()
    {
        var dto = new ProdutoCriacaoDto { Nome = "Teclado", Preco = 0m };
        Assert.ThrowsAsync<DomainException>(() => _service.CriarProdutoAsync(dto));
    }

    [Test]
    public async Task GetProdutoByIdAsync_Existente_DeveRetornarDto()
    {
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Microservice.Produto.Domain.Entities.Produto { Id = 1, Nome = "M", Preco = 10 });
        var r = await _service.GetProdutoByIdAsync(1);
        Assert.That(r, Is.Not.Null);
        Assert.That(r!.Id, Is.EqualTo(1));
    }

    [Test]
    public async Task GetProdutoByIdAsync_Inexistente_DeveRetornarNull()
    {
        _repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Microservice.Produto.Domain.Entities.Produto?)null);
        var r = await _service.GetProdutoByIdAsync(99);
        Assert.That(r, Is.Null);
    }

    [Test]
    public async Task AtualizarAsync_ProdutoExistente_DeveAtualizar()
    {
        var produto = new Microservice.Produto.Domain.Entities.Produto { Id = 1, Nome = "A", Preco = 10 };
        _repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(produto);
        _repo.Setup(r => r.UpdateAsync(produto)).Returns(Task.CompletedTask);
        var dto = new ProdutoAtualizacaoDto { Nome = "B", Preco = 20, Descricao = "Nova" };

        var resp = await _service.AtualizarAsync(1, dto);

        _repo.Verify(r => r.UpdateAsync(produto), Times.Once);
        Assert.That(resp!.Nome, Is.EqualTo("B"));
    }

    [Test]
    public void AtualizarAsync_Inexistente_DeveLancarServiceException()
    {
        _repo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Microservice.Produto.Domain.Entities.Produto?)null);
        var dto = new ProdutoAtualizacaoDto { Nome = "B", Preco = 20 };
        Assert.ThrowsAsync<ServiceException>(() => _service.AtualizarAsync(2, dto));
    }

    [Test]
    public async Task DeletarAsync_Existente_DeveDeletar()
    {
        var produto = new Microservice.Produto.Domain.Entities.Produto { Id = 10, Nome = "A", Preco = 10 };
        _repo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(produto);
        _repo.Setup(r => r.DeleteAsync(produto)).Returns(Task.CompletedTask);
        var ok = await _service.DeletarAsync(10);
        Assert.That(ok, Is.True);
        _repo.Verify(r => r.DeleteAsync(produto), Times.Once);
    }

    [Test]
    public void DeletarAsync_Inexistente_DeveLancarServiceException()
    {
        _repo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Microservice.Produto.Domain.Entities.Produto?)null);
        Assert.ThrowsAsync<ServiceException>(() => _service.DeletarAsync(999));
    }
}
