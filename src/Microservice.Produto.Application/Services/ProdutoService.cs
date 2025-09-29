using AutoMapper;
using Microservice.Produto.Application.DTOs;
using Microservice.Produto.Application.Interfaces;
using Shared.Application.Exceptions;

namespace Microservice.Produto.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _repo;
    private readonly IMapper _mapper;
    private readonly IEstoqueClient _estoqueClient;

    public ProdutoService(IProdutoRepository repo, IMapper mapper, IEstoqueClient estoqueClient)
    {
        this._repo = repo;
        this._mapper = mapper;
        this._estoqueClient = estoqueClient;
    }

    public async Task<ProdutoDto> CriarProdutoAsync(ProdutoCriacaoDto dto)
    {
        Domain.Entities.Produto produto = this._mapper.Map<Microservice.Produto.Domain.Entities.Produto>(dto);
        produto.Validar();

        await this._repo.AddAsync(produto);
        await this._estoqueClient.CriarEstoqueInicialAsync(produto.Id);

        return this._mapper.Map<ProdutoDto>(produto);
    }

    public async Task<ProdutoDto?> GetProdutoByIdAsync(int id)
    {
        Domain.Entities.Produto? produto = await this._repo.GetByIdAsync(id);
        return produto == null ? null : this._mapper.Map<ProdutoDto>(produto);
    }

    public async Task<IEnumerable<ProdutoDto>> GetTodosAsync()
    {
        IEnumerable<Domain.Entities.Produto> produtos = await this._repo.GetAllAsync();
        return produtos.Select(this._mapper.Map<ProdutoDto>);
    }

    public async Task<ProdutoDto?> AtualizarAsync(int id, ProdutoAtualizacaoDto dto)
    {
        Domain.Entities.Produto produto = await this._repo.GetByIdAsync(id) ??
            throw new ServiceException("Ocorreu um erro no servidor.");

        produto.Validar();
        produto.Alterar(dto.Nome, dto.Preco, dto.Descricao);

        await this._repo.UpdateAsync(produto);
        return this._mapper.Map<ProdutoDto>(produto);
    }

    public async Task<bool> DeletarAsync(int id)
    {
        Domain.Entities.Produto? produto = await this._repo.GetByIdAsync(id) ??
            throw new ServiceException("Produto não encontrado.");

        await this._repo.DeleteAsync(produto);
        return true;
    }
}
