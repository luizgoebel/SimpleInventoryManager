using Microservice.Faturamento.Application.Interfaces;
using Microservice.Pedido.Application.Events;
using Microsoft.Extensions.Logging;
using Shared.Messaging;

namespace Microservice.Faturamento.Application.EventHandlers;

public class PedidoCriadoIntegrationEventHandler : IIntegrationEventHandler<PedidoCriadoIntegrationEvent>
{
    private readonly IFaturamentoService _faturamentoService;
    private readonly ILogger<PedidoCriadoIntegrationEventHandler> _logger;

    public PedidoCriadoIntegrationEventHandler(IFaturamentoService faturamentoService, ILogger<PedidoCriadoIntegrationEventHandler> logger)
    {
        _faturamentoService = faturamentoService;
        _logger = logger;
    }

    public async Task HandleAsync(PedidoCriadoIntegrationEvent @event, CancellationToken ct = default)
    {
        try
        {
            await _faturamentoService.FaturarPedidoAsync(@event.PedidoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao faturar pedido {PedidoId} via evento", @event.PedidoId);
        }
    }
}
