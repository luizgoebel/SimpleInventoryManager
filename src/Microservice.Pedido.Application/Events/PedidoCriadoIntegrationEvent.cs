using Shared.Messaging;

namespace Microservice.Pedido.Application.Events;

public record PedidoItemCriadoEventDto(int ProdutoId, int Quantidade, decimal PrecoUnitario);

public record PedidoCriadoIntegrationEvent(int PedidoId, decimal Total, IEnumerable<PedidoItemCriadoEventDto> Itens)
    : IntegrationEvent;
