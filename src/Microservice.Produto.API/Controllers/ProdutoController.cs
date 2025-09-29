using Microsoft.AspNetCore.Mvc;
using Microservice.Produto.Application.Interfaces;
using Microservice.Produto.Application.DTOs;

namespace Microservice.Produto.API.Controllers;

[ApiController]
[Route("api/produtos")]
public class ProdutoController : ControllerBase
{
    private readonly IProdutoService _service;

    public ProdutoController(IProdutoService service)
    {
        _service = service;
    }

    [HttpPost("criar")]
    public async Task<IActionResult> Post([FromBody] ProdutoCriacaoDto dto)
    {
        try
        {
            var produto = await _service.CriarProdutoAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var produto = await _service.GetProdutoByIdAsync(id);
        if (produto == null) return NotFound();
        return Ok(produto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var itens = await _service.GetTodosAsync();
        return Ok(itens);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] ProdutoAtualizacaoDto dto)
    {
        try
        {
            var atualizado = await _service.AtualizarAsync(id, dto);
            if (atualizado == null) return NotFound();
            return Ok(atualizado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeletarAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}
