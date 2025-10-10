using Microsoft.AspNetCore.Mvc;
using Microservice.Estoque.Application.Interfaces;
using Microservice.Estoque.Application.DTOs;

namespace Microservice.Estoque.API.Controllers;

[ApiController]
[Route("api/estoque")]
public class EstoqueController : ControllerBase
{
    private readonly IEstoqueService _service;

    public EstoqueController(IEstoqueService service)
    {
        _service = service;
    }

    [HttpGet("{produtoId:int}")]
    public async Task<IActionResult> Get(int produtoId)
    {
        var estoque = await _service.GetPorProdutoIdAsync(produtoId);
        if (estoque == null) return NotFound();
        return Ok(estoque);
    }

    [HttpPost("entrada")]
    public async Task<IActionResult> Entrada([FromBody] MovimentoEstoqueDto dto)
    {
        var ok = await _service.EntradaAsync(dto);
        if (!ok) return BadRequest();
        return Ok();
    }

    [HttpPost("saida")]
    public async Task<IActionResult> Saida([FromBody] MovimentoEstoqueDto dto)
    {
        var ok = await _service.SaidaAsync(dto);
        if (!ok) return BadRequest();
        return Ok();
    }

    [HttpPost("inicial/{produtoId:int}")]
    public async Task<IActionResult> Inicial(int produtoId)
    {
        await _service.CriarInicialSeNaoExisteAsync(produtoId);
        return Ok();
    }
}
