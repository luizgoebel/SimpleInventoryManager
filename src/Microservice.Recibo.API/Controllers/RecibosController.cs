using Microservice.Recibo.Application.DTOs;
using Microservice.Recibo.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Recibo.API.Controllers;

[ApiController]
[Route("api/recibos")]
public class RecibosController : ControllerBase
{
    private readonly IReciboService _service;

    public RecibosController(IReciboService service)
    {
        this._service = service;
    }

    [HttpPost("emissao")]
    public async Task<IActionResult> Emitir([FromBody] EmissaoReciboDto emissaoReciboDto)
    {
        ReciboDto reciboDto =
            await this._service.GerarPorFaturaAsync(emissaoReciboDto.FaturaId, emissaoReciboDto.NumeroFatura, emissaoReciboDto.ValorTotal);
        return CreatedAtAction(nameof(GetPorFatura), new { faturaId = emissaoReciboDto.FaturaId }, reciboDto);
    }

    [HttpGet("fatura/{faturaId:int}")]
    public async Task<IActionResult> GetPorFatura(int faturaId)
    {
        ReciboDto? reciboDto = await this._service.ObterPorFaturaAsync(faturaId);
        return Ok(reciboDto);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        ReciboDto? reciboDto = await this._service.ObterPorIdAsync(id);
        return Ok(reciboDto);
    }
}
