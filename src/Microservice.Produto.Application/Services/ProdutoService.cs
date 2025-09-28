using AutoMapper;
using Microservice.Produto.Application.DTOs;
using Microservice.Produto.Application.Interfaces;

namespace Microservice.Produto.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _repo;
    private readonly IMapper _mapper;
    private readonly IEstoqueClient _estoqueClient;

    public ProdutoService(IProdutoRepository repo, IMapper mapper, IEstoqueClient estoqueClient)
    {
        _repo = repo;
        _mapper = mapper;
        _estoqueClient = estoqueClient;
    }

    public async Task<ProdutoDto> CriarProdutoAsync(ProdutoCriacaoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome)) throw new ArgumentException("Nome é obrigatório");
        if (dto.Preco <= 0) throw new ArgumentException("Preço deve ser maior que zero");

        var entidade = _mapper.Map<Microservice.Produto.Domain.Entities.Produto>(dto);
        await _repo.AddAsync(entidade);

        // Chamar estoque para criar registro inicial
        await _estoqueClient.CriarEstoqueInicialAsync(entidade.Id);

        return _mapper.Map<ProdutoDto>(entidade);
    }

    public async Task<ProdutoDto?> GetProdutoByIdAsync(int id)
    {
        var entidade = await _repo.GetByIdAsync(id);
        return entidade == null ? null : _mapper.Map<ProdutoDto>(entidade);
    }

    public async Task<IEnumerable<ProdutoDto>> GetTodosAsync()
    {
        var itens = await _repo.GetAllAsync();
        return itens.Select(_mapper.Map<ProdutoDto>);
    }

    public async Task<ProdutoDto?> AtualizarAsync(int id, ProdutoAtualizacaoDto dto)
    {
        var entidade = await _repo.GetByIdAsync(id);
        if (entidade == null) return null;
        if (string.IsNullOrWhiteSpace(dto.Nome)) throw new ArgumentException("Nome é obrigatório");
        if (dto.Preco <= 0) throw new ArgumentException("Preço deve ser maior que zero");

        entidade.Nome = dto.Nome;
        entidade.Preco = dto.Preco;
        entidade.Descricao = dto.Descricao;

        await _repo.UpdateAsync(entidade);
        return _mapper.Map<ProdutoDto>(entidade);
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var entidade = await _repo.GetByIdAsync(id);
        if (entidade == null) return false;
        await _repo.DeleteAsync(entidade);
        return true;
    }
}
