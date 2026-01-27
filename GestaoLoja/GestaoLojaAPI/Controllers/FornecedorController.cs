using RCLGeral.Entities;
using GestaoLoja.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GestaoLojaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FornecedorController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FornecedorController> _logger;

    public FornecedorController(ApplicationDbContext context, ILogger<FornecedorController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/Fornecedor/categorias
    [HttpGet("categorias")]
    public async Task<IActionResult> GetCategorias()
    {
        var categorias = await _context.Categorias
            .OrderBy(c => c.Ordem)
            .ThenBy(c => c.Nome)
            .ToListAsync();
        
        return Ok(categorias);
    }

    // GET: api/Fornecedor/modos-disponibilizacao
    [HttpGet("modos-disponibilizacao")]
    public async Task<IActionResult> GetModosDisponibilizacao()
    {
        var modos = await _context.ModoEntregas
            .OrderBy(m => m.Nome)
            .ToListAsync();
        
        return Ok(modos);
    }

    // GET: api/Fornecedor/produtos
    [HttpGet("produtos")]
    [Authorize(Roles = "Fornecedor")] // removi temporariamente "Roles = Fornecedor" para testar umas merdas
    public async Task<IActionResult> GetMeusProdutos()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        // Log all claims for debugging
        var allClaims = User.Claims.Select(c => $"{c.Type}: {c.Value}");
        Console.WriteLine($"All Claims: {string.Join(" | ", allClaims)}");
        Console.WriteLine($"UserId: {userId}");
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "UserId not found in token" });
        }

        var produtos = await _context.Produtos
            .Where(p => p.FornecedorId == userId)
            .Include(p => p.categoria)
            .Include(p => p.modoentrega)
            .OrderByDescending(p => p.Id)
            .ToListAsync();

        Console.WriteLine($"Produtos encontrados: {produtos.Count}");

        return Ok(produtos);
    }

    // GET: api/Fornecedor/produtos/{id}
    [HttpGet("produtos/{id}")]
    [Authorize(Roles = "Fornecedor")]
    public async Task<IActionResult> GetProduto(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var produto = await _context.Produtos
            .Include(p => p.categoria)
            .Include(p => p.modoentrega)
            .Include(p => p.Fornecedor)
            .FirstOrDefaultAsync(p => p.Id == id && p.FornecedorId == userId);

        if (produto == null)
        {
            return NotFound();
        }

        return Ok(produto);
    }

    // POST: api/Fornecedor/produtos
    [HttpPost("produtos")]
    [Authorize(Roles = "Fornecedor")]
    public async Task<IActionResult> CriarProduto([FromBody] CriarProdutoRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        _logger.LogInformation($"CriarProduto - UserId: '{userId}'");
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Utilizador nao autenticado" });
        }

        var produto = new Produto
        {
            Nome = request.Nome,
            Detalhe = request.Detalhe,
            PrecoBase = request.PrecoBase,
            Stock = (int)request.Stock,
            UrlImagem = request.UrlImagem,
            ModoEntregaId = request.ModoDisponibilizacaoId,
            FornecedorId = userId.Trim(),
            Ativo = false,
            Disponivel = true,
            PercentagemLucro = 0,
            Promocao = false,
            MaisVendido = false
        };

        if (!string.IsNullOrEmpty(request.ImagemBase64))
        {
            produto.Imagem = Convert.FromBase64String(request.ImagemBase64);
        }

        if (request.CategoriaIds != null && request.CategoriaIds.Any())
        {
            produto.CategoriaId = request.CategoriaIds.First();
        }

        produto.CalcularPrecoFinal();

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation($"Produto criado - Id: {produto.Id}, FornecedorId: '{produto.FornecedorId}'");

        return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto);
    }

    // PUT: api/Fornecedor/produtos/{id}
    [HttpPut("produtos/{id}")]
    [Authorize(Roles = "Fornecedor")]
    public async Task<IActionResult> EditarProduto(int id, [FromBody] CriarProdutoRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var produto = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id && p.FornecedorId == userId);

        if (produto == null)
        {
            return NotFound();
        }

        produto.Nome = request.Nome;
        produto.Detalhe = request.Detalhe;
        produto.PrecoBase = request.PrecoBase;
        produto.Stock = (int)request.Stock;
        produto.UrlImagem = request.UrlImagem;
        produto.ModoEntregaId = request.ModoDisponibilizacaoId;

        if (!string.IsNullOrEmpty(request.ImagemBase64))
        {
            produto.Imagem = Convert.FromBase64String(request.ImagemBase64);
        }

        if (request.CategoriaIds != null && request.CategoriaIds.Any())
        {
            produto.CategoriaId = request.CategoriaIds.First();
        }

        produto.CalcularPrecoFinal();
        produto.Ativo = false;

        await _context.SaveChangesAsync();

        return Ok(produto);
    }

    // DELETE: api/Fornecedor/produtos/{id}
    [HttpDelete("produtos/{id}")]
    [Authorize(Roles = "Fornecedor")]
    public async Task<IActionResult> ApagarProduto(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var produto = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id && p.FornecedorId == userId);

        if (produto == null)
        {
            return NotFound();
        }

        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Fornecedor/produtos/{id}/suspender
    [HttpPost("produtos/{id}/suspender")]
    [Authorize(Roles = "Fornecedor")]
    public async Task<IActionResult> SuspenderProduto(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var produto = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id && p.FornecedorId == userId);

        if (produto == null)
        {
            return NotFound();
        }

        produto.Disponivel = false;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Produto suspenso com sucesso" });
    }

    // POST: api/Fornecedor/produtos/{id}/reativar
    [HttpPost("produtos/{id}/reativar")]
    [Authorize(Roles = "Fornecedor")]
    public async Task<IActionResult> ReativarProduto(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var produto = await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id && p.FornecedorId == userId);

        if (produto == null)
        {
            return NotFound();
        }

        produto.Disponivel = true;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Produto reativado com sucesso" });
    }

    // GET: api/Fornecedor/vendas
    [HttpGet("vendas")]
    [Authorize(Roles = "Fornecedor")]
    public async Task<IActionResult> GetVendas()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        _logger.LogInformation($"GetVendas - UserId: '{userId}'");

        // Obter todos os IDs de produtos do fornecedor
        var produtosDoFornecedor = await _context.Produtos
            .Where(p => p.FornecedorId == userId)
            .Select(p => p.Id)
            .ToListAsync();

        _logger.LogInformation($"Produtos do fornecedor: {produtosDoFornecedor.Count}");

        if (!produtosDoFornecedor.Any())
        {
            return Ok(new List<object>());
        }

        // Obter pedidos que contêm produtos deste fornecedor
        var pedidosDoFornecedor = await _context.Pedidos
            .Where(p => produtosDoFornecedor.Contains(p.ProdutoId))
            .Include(p => p.Encomenda)
            .ToListAsync();

        _logger.LogInformation($"Pedidos do fornecedor: {pedidosDoFornecedor.Count}");

        // Agrupar por encomenda
        var encomendaIds = pedidosDoFornecedor
            .Where(p => p.EncomendaId.HasValue)
            .Select(p => p.EncomendaId!.Value)
            .Distinct()
            .ToList();

        // Obter as encomendas completas
        var encomendas = await _context.Encomendas
            .Where(e => encomendaIds.Contains(e.Id))
            .Include(e => e.Pedidos)
            .OrderByDescending(e => e.DataCriacao)
            .ToListAsync();

        _logger.LogInformation($"Encomendas encontradas: {encomendas.Count}");

        // Obter informações dos produtos
        var todosProdutos = await _context.Produtos
            .Where(p => produtosDoFornecedor.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        // Mapear para o formato esperado pelo frontend (EncomendaModel)
        var resultado = encomendas.Select(e => new
        {
            id = e.Id,
            dataCriacao = e.DataCriacao ?? DateTime.Now,
            dataAtualizacao = (DateTime?)null,
            dataPagamento = e.EstadoPagamento == "Pago" ? e.DataCriacao : (DateTime?)null,
            dataExpedicao = e.EstadoEncomenda == "Expedido" ? e.DataCriacao : (DateTime?)null,
            estado = MapearEstado(e.EstadoEncomenda, e.EstadoPagamento),
            total = e.Pedidos
                .Where(p => produtosDoFornecedor.Contains(p.ProdutoId))
                .Sum(p => p.ValorTotal),
            metodoPagamento = (string?)null,
            referenciaPagamento = (string?)null,
            itens = e.Pedidos
                .Where(p => produtosDoFornecedor.Contains(p.ProdutoId))
                .Select(p => new
                {
                    id = p.Id,
                    produtoId = p.ProdutoId,
                    produtoNome = todosProdutos.ContainsKey(p.ProdutoId) 
                        ? todosProdutos[p.ProdutoId].Nome ?? "Produto" 
                        : "Produto",
                    produtoImagem = todosProdutos.ContainsKey(p.ProdutoId) && todosProdutos[p.ProdutoId].Imagem != null
                        ? $"data:image/jpeg;base64,{Convert.ToBase64String(todosProdutos[p.ProdutoId].Imagem!)}"
                        : (string?)null,
                    quantidade = p.Quantidade,
                    precoUnitario = p.PrecoUnitario
                }).ToList()
        }).ToList();

        return Ok(resultado);
    }

    private static string MapearEstado(string estadoEncomenda, string estadoPagamento)
    {
        if (estadoEncomenda == "Expedido" && estadoPagamento == "Pago")
            return "Concluida";
        if (estadoEncomenda == "Expedido")
            return "Expedida";
        if (estadoPagamento == "Pago")
            return "Paga";
        if (estadoEncomenda == "Pendente")
            return "Confirmada";
        return "Pendente";
    }
}

public class CriarProdutoRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Detalhe { get; set; } = string.Empty;
    public decimal PrecoBase { get; set; }
    public decimal Stock { get; set; }
    public string? UrlImagem { get; set; }
    public string? ImagemBase64 { get; set; }
    public int ModoDisponibilizacaoId { get; set; }
    public List<int> CategoriaIds { get; set; } = new();
}