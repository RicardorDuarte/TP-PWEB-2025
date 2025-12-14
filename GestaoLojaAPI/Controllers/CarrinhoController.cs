using GestaoLojaAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using GestaoLojaAPI.Data;
using RCLAPI.DTO;
using GestaoLojaAPI.Repositories;

namespace GestaoLojaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarrinhoController : ControllerBase
{
    private readonly ICarrinhoRepository _carrinhoRepository;

    public CarrinhoController(ICarrinhoRepository carrinhoRepository)
    {
        _carrinhoRepository = carrinhoRepository;
    }

    [HttpPost("adicionarOuAtualizar")]
    public async Task<IActionResult> AdicionarOuAtualizarItem([FromBody] ItemCarrinhoCompra item)
    {
        if (item == null)
        {
            return BadRequest("Item inválido.");
        }

        var resultado = await _carrinhoRepository.AdicionarOuAtualizarItem(item);

        if (resultado)
        {
            return Ok(new { mensagem = "Item adicionado ou atualizado com sucesso." });
        }

        return StatusCode(500, "Erro ao processar a requisição.");
    }


    [HttpGet("obterCarrinho/{userId}")]
    public async Task<IActionResult> GetCarrinho(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("Utilizador não especificado.");
        }

        var carrinho = await _carrinhoRepository.ObterCarrinhoPorUser(userId);

        if (carrinho == null || !carrinho.Any())
        {
            return NotFound("Carrinho vazio ou não encontrado.");
        }

        return Ok(carrinho);
    }

    [HttpDelete("remover/{id}")]
    public async Task<IActionResult> RemoverItem(int id)
    {
        var resultado = await _carrinhoRepository.RemoverItem(id);
        if (resultado)
        {
            return Ok(new { mensagem = "Item removido com sucesso." });
        }

        return NotFound(new { mensagem = "Item não encontrado." });
    }    

    [HttpPost("atualizar")]
    public async Task<IActionResult> AtualizarItemCarrinho([FromBody] ItemCarrinhoCompra item)
    {
        if (item == null)
        {
            return BadRequest("Item inválido.");
        }

        var resultado = await _carrinhoRepository.AtualizarItem(item);

        if (resultado)
        {
            return Ok(new { mensagem = "Item adicionado ou atualizado com sucesso." });
        }

        return StatusCode(500, "Erro ao processar a requisição.");
    }

}

