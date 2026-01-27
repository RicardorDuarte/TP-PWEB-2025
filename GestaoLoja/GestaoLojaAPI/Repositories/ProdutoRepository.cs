using Microsoft.EntityFrameworkCore;
using GestaoLoja.Data;       // Changed from GestaoLojaAPI.Context
using RCLGeral.Entities;   // Changed from GestaoLojaAPI.Entities


namespace GestaoLojaAPI.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly ApplicationDbContext _context;  // Changed from AppDbContext

    public ProdutoRepository(ApplicationDbContext context)  // Changed from AppDbContext
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Produto>> ObterProdutosPorCategoriaAsync(int categoriaId)
    {
        return await _context.Produtos
            .Where(p => p.CategoriaId == categoriaId)
            .Where(x => x.Imagem.Length > 0)
            .Include("modoentrega")
            .Include("categoria")
            .OrderBy(o => o.Nome)
            .ToListAsync();
    }
    public async Task<IEnumerable<Produto>> ObterProdutosPromocaoAsync()
    {
        return await _context.Produtos
            .Where(p => p.Promocao == true)
            .Where(x => x.Imagem!.Length > 0)
            .Include("modoentrega")
            .Include("categoria")
            .OrderBy(p => p.categoria.Ordem)
            .ThenBy(p => p.Nome)
            .ToListAsync();
    }
    public async Task<IEnumerable<Produto>> ObterProdutosMaisVendidosAsync()
    {
        return await _context.Produtos
            .Where(p => p.MaisVendido)
            .Where(x => x.Imagem!.Length > 0)
            .Include("modoentrega")
            .Include("categoria")
            .OrderBy(p => p.categoria.Ordem)
            .ThenBy(p => p.Nome)
            .ToListAsync();
    }
    public async Task<IEnumerable<Produto>> ObterTodosProdutosAsync()
    {
        var produtos = await _context.Produtos
            .Where(x => x.Imagem!.Length > 0)
            .Include("modoentrega")
            .Include("categoria")
            .OrderBy(p => p.categoria.Ordem)
            .ThenBy(p => p.Nome)
            .ToListAsync();

        return produtos;
    }
    public async Task<Produto> ObterDetalheProdutoAsync(int id)
    {
        var detalheProduto = await _context.Produtos
            .Where(x => x.Imagem!.Length > 0)
            .Include("modoentrega")
            .Include("categoria")
            .FirstOrDefaultAsync(p => p.Id == id);

        if (detalheProduto is null)
            throw new InvalidOperationException();

        return detalheProduto;
    }

    public async Task<Produto?> GetProdutoByIdAsync(int id)
    {
        // Busca o produto pelo ID, garantindo que tenha imagem e incluindo as relações necessárias.
        return await _context.Produtos
            .Where(p => p.Id == id)
            .Where(p => p.Imagem!.Length > 0)
            .Include("modoentrega")
            .Include("categoria")
            .FirstOrDefaultAsync();
    }
}
