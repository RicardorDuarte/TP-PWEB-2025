using GestaoLojaAPI.Entities;
using GestaoLojaAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GestaoLojaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

//[Authorized]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaRepository categoriaRepository;

    public CategoriasController(ICategoriaRepository categoriaRepository)
    {
        this.categoriaRepository = categoriaRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var categorias = await categoriaRepository.GetCategorias();
        return Ok(categorias);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var categorias = await categoriaRepository.GetCategorias();
        var categoria = categorias.FirstOrDefault(c => c.Id == id);
        
        if (categoria == null)
            return NotFound($"Categoria com id={id} não encontrada");
            
        return Ok(categoria);
    }
}
