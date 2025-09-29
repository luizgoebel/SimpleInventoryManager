using AutoMapper;
using Microservice.Estoque.Application.DTOs;
using Microservice.Estoque.Application.Interfaces;
using Shared.Application.Exceptions;

namespace Microservice.Estoque.Application.Services;

public class EstoqueService : IEstoqueService
{
    private readonly IEstoqueRepository _repo;
    private readonly IMapper _mapper;

    public EstoqueService(IEstoqueRepository repo, IMapper mapper)
    {
        this._repo = repo;
        this._mapper = mapper;
    }

    public async Task<bool> CriarInicialSeNaoExisteAsync(int produtoId)
    {
        Domain.Entities.Estoque? existente = await this._repo.GetByProdutoIdAsync(produtoId);
        if (existente != null) return true;

        Domain.Entities.Estoque estoqueInicial = new() { ProdutoId = produtoId, Quantidade = 0 };
        await this._repo.AddAsync(estoqueInicial);
        return true;
    }

    public async Task<EstoqueDto?> GetPorProdutoIdAsync(int produtoId)
    {
        Domain.Entities.Estoque? estoque = await this._repo.GetByProdutoIdAsync(produtoId);
        return estoque == null ? null : this._mapper.Map<EstoqueDto>(estoque);
    }

    public async Task<bool> EntradaAsync(MovimentoEstoqueDto dto)
    {
        Domain.Entities.Estoque? estoqueExistente = await this._repo.GetByProdutoIdAsync(dto.ProdutoId);
        if (estoqueExistente == null)
        {
            estoqueExistente = new Microservice.Estoque.Domain.Entities.Estoque { ProdutoId = dto.ProdutoId, Quantidade = 0 };
            await this._repo.AddAsync(estoqueExistente);
        }

        ValidarQuantidade(dto.Quantidade);
        estoqueExistente.AdicionarQuantidade(dto.Quantidade);
        await this._repo.UpdateAsync(estoqueExistente);
        return true;
    }

    public async Task<bool> SaidaAsync(MovimentoEstoqueDto dto)
    {
        ValidarQuantidade(dto.Quantidade);
        Domain.Entities.Estoque? estoqueExistente = await this._repo.GetByProdutoIdAsync(dto.ProdutoId) ??
            throw new ServiceException("Estoque inexistente.");
        if (estoqueExistente.Quantidade - dto.Quantidade < 0) throw new ServiceException("Quantidade de saída é maior que o estoque.");
        estoqueExistente.SubtrairQuantidade(dto.Quantidade);
        await this._repo.UpdateAsync(estoqueExistente);
        return true;
    }

    private void ValidarQuantidade(int quantidade)
    {
        if (quantidade < 0) throw new ServiceException("Quantidade não pode ser menor que 0.");
    }
}
