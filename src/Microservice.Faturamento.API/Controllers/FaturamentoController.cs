using Microsoft.AspNetCore.Mvc;
using Microservice.Faturamento.Application.Interfaces;
using Shared.Application.Exceptions;

namespace Microservice.Faturamento.API.Controllers;

[ApiController]
[Route("api/faturamento")]
public class FaturamentoController : ControllerBase
{
    private readonly IFaturamentoService _service;
    public FaturamentoController(IFaturamentoService service) => _service = service;

    [HttpPost("{pedidoId:int}")]
    public async Task<IActionResult> Faturar(int pedidoId)
    {
        try
        {
            var result = await _service.FaturarPedidoAsync(pedidoId);
            return CreatedAtAction(nameof(GetPorPedido), new { pedidoId }, result);
        }
        catch (ServiceException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpGet("pedido/{pedidoId:int}")]
    public async Task<IActionResult> GetPorPedido(int pedidoId)
    {
        var dto = await _service.ObterPorPedidoAsync(pedidoId);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var dto = await _service.ObterPorIdAsync(id);
        return dto == null ? NotFound() : Ok(dto);
    }
}
