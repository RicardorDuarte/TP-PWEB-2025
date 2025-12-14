using Microsoft.AspNetCore.Mvc;

using GestaoLojaAPI.Entities;
using GestaoLojaAPI.Repositories;

namespace GestaoLojaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

//[Authorize]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoRepository _produtoRepository;

    public ProdutosController(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetProdutos(string tipoProduto, int? categoriaId = null)
    {
        IEnumerable<Produto> produtos;

        if (tipoProduto == "categoria" && categoriaId != null)
        {
            produtos = await _produtoRepository.
                ObterProdutosPorCategoriaAsync(categoriaId.Value);
        }
        else if (tipoProduto == "detalhe" && categoriaId != null)
        {
            Produto produto = await _produtoRepository.
                ObterDetalheProdutoAsync(categoriaId.Value);

            return Ok(produto);
        }
        else if (tipoProduto == "promocao")
        {
            var promocoes = await _produtoRepository.ObterProdutosPromocaoAsync();

            return Ok(promocoes);
        }
        else if (tipoProduto == "maisvendido")
        {
            produtos = await _produtoRepository.ObterProdutosMaisVendidosAsync();
        }
        else if (tipoProduto == "todos")
        {
            produtos = await _produtoRepository.ObterTodosProdutosAsync();
        }
        else
        {
            return BadRequest("Tipo de produto inválido");
        }

        return Ok(produtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetalheProduto(int id)
    {
        Produto produto = await _produtoRepository.ObterDetalheProdutoAsync(id);

        if (produto is null)
        {
            return NotFound($"Produto com id={id} não encontrado");
        }
        return Ok(produto);
    }
}
