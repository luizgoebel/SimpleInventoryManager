using AutoMapper;
using Microservice.Pedido.Application.DTOs;
using Microservice.Pedido.Application.Interfaces;
using Microservice.Pedido.Domain.Entities;
using Shared.Application.Exceptions;

namespace Microservice.Pedido.Application.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _repo;
    private readonly IMapper _mapper;
    private readonly IEstoqueMovimentoClient _estoqueClient;

    public PedidoService(IPedidoRepository repo, IMapper mapper, IEstoqueMovimentoClient estoqueClient)
    {
        this._repo = repo;
        this._mapper = mapper;
        this._estoqueClient = estoqueClient;
    }

    public async Task<PedidoDto> CriarAsync(PedidoCriacaoDto dto)
    {
        if (dto.Itens == null || !dto.Itens.Any())
            throw new ServiceException("Por favor, inserir itens no pedido.");

        Microservice.Pedido.Domain.Entities.Pedido pedido = new();

        foreach (PedidoItemCriacaoDto item in dto.Itens)
        {
            if (item.ProdutoId <= 0) throw new ServiceException("Por favor, inserir produto válido no pedido.");
            if (item.Quantidade <= 0) throw new ServiceException("Quantidade inválida.");
            if (item.PrecoUnitario < 0) throw new ServiceException("Preço inválido.");
            pedido.AdicionarItem(item.ProdutoId, item.Quantidade, item.PrecoUnitario);
        }

        pedido.Validar();

        foreach (PedidoItem item in pedido.Itens)
            await this._estoqueClient.RegistrarSaidaAsync(item.ProdutoId, item.Quantidade);

        await this._repo.AddAsync(pedido);
        return this._mapper.Map<PedidoDto>(pedido);
    }

    public async Task<PedidoDto?> GetByIdAsync(int id)
    {
        Domain.Entities.Pedido? pedido = await _repo.GetByIdWithItemsAsync(id) ??
                       throw new ServiceException("Pedido não encontrado.");
        return _mapper.Map<PedidoDto>(pedido);
    }

    public async Task<IEnumerable<PedidoDto>> GetTodosAsync()
    {
        IEnumerable<Domain.Entities.Pedido> pedidos = await _repo.GetAllAsync();
        return pedidos.Select(_mapper.Map<PedidoDto>);
    }

    public async Task<bool> CancelarAsync(int id)
    {
        Domain.Entities.Pedido? pedido = await _repo.GetByIdWithItemsAsync(id) ??
            throw new ServiceException("Pedido não encontrado.");

        try
        {
            pedido.Cancelar();
        }
        catch (Exception ex)
        {
            throw new ServiceException(ex.Message);
        }

        await _repo.UpdateAsync(pedido);
        return true;
    }
}
