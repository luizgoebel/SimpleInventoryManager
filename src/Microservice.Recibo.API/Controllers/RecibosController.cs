using Microsoft.AspNetCore.Mvc;
using Microservice.Recibo.Application.Interfaces;
using Shared.Application.Exceptions;

namespace Microservice.Recibo.API.Controllers;

[ApiController]
[Route("api/recibos")]
public class RecibosController : ControllerBase
{
    private readonly IReciboService _service;

    public RecibosController(IReciboService service) => _service = service;

    public record EmissaoReciboRequest(int FaturaId, string NumeroFatura, decimal ValorTotal);

    [HttpPost("emissao")]
    public async Task<IActionResult> Emitir([FromBody] EmissaoReciboRequest request)
    {
        try
        {
            var dto = await _service.GerarPorFaturaAsync(request.FaturaId, request.NumeroFatura, request.ValorTotal);
            return CreatedAtAction(nameof(GetPorFatura), new { faturaId = request.FaturaId }, dto);
        }
        catch (ServiceException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpGet("fatura/{faturaId:int}")]
    public async Task<IActionResult> GetPorFatura(int faturaId)
    {
        var dto = await _service.ObterPorFaturaAsync(faturaId);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var dto = await _service.ObterPorIdAsync(id);
        return dto == null ? NotFound() : Ok(dto);
    }
}
