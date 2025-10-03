using AutoMapper;
using Microservice.Produto.Application.DTOs;
using Microservice.Produto.Application.Interfaces;
using Microservice.Produto.Application.Mapping;
using Microservice.Produto.Application.Services;
using Shared.Application.Exceptions;
using Shared.Domain.Exceptions;

namespace Microservice.Produto.Application.Tests.ProdutoServiceTests;

public class ProdutoServiceTests
{
    private Mock<IProdutoRepository> _repo = null!;
    private Mock<IEstoqueClient> _estoqueClient = null!;
    private IMapper _mapper = null!;
    private ProdutoService _service = null!;

    [SetUp]
    public void SetUp()
    {
        this._repo = new Mock<IProdutoRepository>(MockBehavior.Strict);
        this._estoqueClient = new Mock<IEstoqueClient>(MockBehavior.Loose);
        MapperConfiguration cfg = new MapperConfiguration(c => c.AddProfile<ProdutoProfile>());
        this._mapper = cfg.CreateMapper();
        this._service = new ProdutoService(this._repo.Object, this._mapper, this._estoqueClient.Object);
    }

    [Test]
    public async Task CriarProdutoAsync_DadosValidos_DeveCriarProduto()
    {
        ProdutoCriacaoDto dto = new ProdutoCriacaoDto { Nome = "Teclado", Preco = 100m, Descricao = "Desc" };

        this._repo.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Produto>())).Returns(Task.CompletedTask);
        this._estoqueClient.Setup(e => e.CriarEstoqueInicialAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

        ProdutoDto result = await this._service.CriarProdutoAsync(dto);

        this._repo.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Produto>()), Times.Once);
        this._estoqueClient.Verify(e => e.CriarEstoqueInicialAsync(It.IsAny<int>()), Times.Once);
        Assert.That(result.Nome, Is.EqualTo(dto.Nome));
    }

    [Test]
    public void CriarProdutoAsync_PrecoInvalido_DeveLancarDomainException()
    {
        ProdutoCriacaoDto dto = new ProdutoCriacaoDto { Nome = "Teclado", Preco = 0m };
        Assert.ThrowsAsync<DomainException>(() => this._service.CriarProdutoAsync(dto));
    }

    [Test]
    public async Task GetProdutoByIdAsync_Existente_DeveRetornarDto()
    {
        this._repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Domain.Entities.Produto { Id = 1, Nome = "M", Preco = 10 });
        ProdutoDto? r = await this._service.GetProdutoByIdAsync(1);
        Assert.That(r, Is.Not.Null);
        Assert.That(r!.Id, Is.EqualTo(1));
    }

    [Test]
    public async Task GetProdutoByIdAsync_Inexistente_DeveRetornarNull()
    {
        this._repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Domain.Entities.Produto?)null);
        ProdutoDto? r = await this._service.GetProdutoByIdAsync(99);
        Assert.That(r, Is.Null);
    }

    [Test]
    public async Task AtualizarAsync_ProdutoExistente_DeveAtualizar()
    {
        Domain.Entities.Produto produto = new Domain.Entities.Produto { Id = 1, Nome = "A", Preco = 10 };
        this._repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(produto);
        this._repo.Setup(r => r.UpdateAsync(produto)).Returns(Task.CompletedTask);
        ProdutoAtualizacaoDto dto = new ProdutoAtualizacaoDto { Nome = "B", Preco = 20, Descricao = "Nova" };

        ProdutoDto? resp = await this._service.AtualizarAsync(1, dto);

        this._repo.Verify(r => r.UpdateAsync(produto), Times.Once);
        Assert.That(resp!.Nome, Is.EqualTo("B"));
    }

    [Test]
    public void AtualizarAsync_Inexistente_DeveLancarServiceException()
    {
        this._repo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Domain.Entities.Produto?)null);
        ProdutoAtualizacaoDto dto = new ProdutoAtualizacaoDto { Nome = "B", Preco = 20 };
        Assert.ThrowsAsync<ServiceException>(() => this._service.AtualizarAsync(2, dto));
    }

    [Test]
    public async Task DeletarAsync_Existente_DeveDeletar()
    {
        Domain.Entities.Produto produto = new Domain.Entities.Produto { Id = 10, Nome = "A", Preco = 10 };
        this._repo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(produto);
        this._repo.Setup(r => r.DeleteAsync(produto)).Returns(Task.CompletedTask);
        bool ok = await this._service.DeletarAsync(10);
        Assert.That(ok, Is.True);
        this._repo.Verify(r => r.DeleteAsync(produto), Times.Once);
    }

    [Test]
    public void DeletarAsync_Inexistente_DeveLancarServiceException()
    {
        this._repo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Domain.Entities.Produto?)null);
        Assert.ThrowsAsync<ServiceException>(() => this._service.DeletarAsync(999));
    }
}
