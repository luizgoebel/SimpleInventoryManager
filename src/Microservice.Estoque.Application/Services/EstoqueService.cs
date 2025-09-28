using AutoMapper;
using Microservice.Estoque.Application.DTOs;
using Microservice.Estoque.Application.Interfaces;

namespace Microservice.Estoque.Application.Services;

public class EstoqueService : IEstoqueService
{
    private readonly IEstoqueRepository _repo;
    private readonly IMapper _mapper;

    public EstoqueService(IEstoqueRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<bool> CriarInicialSeNaoExisteAsync(int produtoId)
    {
        var existente = await _repo.GetByProdutoIdAsync(produtoId);
        if (existente != null) return true;
        var estoque = new Microservice.Estoque.Domain.Entities.Estoque { ProdutoId = produtoId, Quantidade = 0 };
        await _repo.AddAsync(estoque);
        return true;
    }

    public async Task<EstoqueDto?> GetPorProdutoIdAsync(int produtoId)
    {
        var entidade = await _repo.GetByProdutoIdAsync(produtoId);
        return entidade == null ? null : _mapper.Map<EstoqueDto>(entidade);
    }

    public async Task<bool> EntradaAsync(MovimentoEstoqueDto dto)
    {
        if (dto.Quantidade < 0) return false;
        var existente = await _repo.GetByProdutoIdAsync(dto.ProdutoId);
        if (existente == null)
        {
            existente = new Microservice.Estoque.Domain.Entities.Estoque { ProdutoId = dto.ProdutoId, Quantidade = 0 };
            await _repo.AddAsync(existente);
        }
        existente.Quantidade += dto.Quantidade;
        await _repo.UpdateAsync(existente);
        return true;
    }

    public async Task<bool> SaidaAsync(MovimentoEstoqueDto dto)
    {
        if (dto.Quantidade < 0) return false;
        var existente = await _repo.GetByProdutoIdAsync(dto.ProdutoId);
        if (existente == null) return false;
        if (existente.Quantidade - dto.Quantidade < 0) return false;
        existente.Quantidade -= dto.Quantidade;
        await _repo.UpdateAsync(existente);
        return true;
    }
}
