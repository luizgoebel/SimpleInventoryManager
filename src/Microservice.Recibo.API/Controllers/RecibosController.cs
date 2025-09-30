using Microsoft.AspNetCore.Mvc;
using Microservice.Recibo.Application.Interfaces;
using Shared.Application.Exceptions;

namespace Microservice.Recibo.API.Controllers;

[ApiController]
[Route("api/recibos")]
public class RecibosController : ControllerBase
{
    private readonly IReciboService _service;

    public RecibosController(IReciboService service)
    {
        _service = service;
    }

    [HttpPost("emissao")]
    public async Task<IActionResult> Emitir(dynamic body)
    {
        try
        {
            int faturaId = (int)body.faturaId;
            string numeroFatura = (string)body.numeroFatura;
            decimal valorTotal = (decimal)body.valorTotal;

            var dto = await _service.GerarPorFaturaAsync(faturaId, numeroFatura, valorTotal);
            return CreatedAtAction(nameof(GetPorFatura), new { faturaId }, dto);
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
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var dto = await _service.ObterPorIdAsync(id);
        if (dto == null) return NotFound();
        return Ok(dto);
    }
}
