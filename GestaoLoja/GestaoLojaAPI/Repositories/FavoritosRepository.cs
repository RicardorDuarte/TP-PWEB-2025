using Microsoft.EntityFrameworkCore;
using GestaoLoja.Data;       // Changed from GestaoLojaAPI.Context and GestaoLojaAPI.Data
using RCLGeral.Entities;   // Changed from GestaoLojaAPI.Entities

namespace GestaoLojaAPI.Repositories;

public class FavoritosRepository : IFavoritosRepository
{
    private readonly ApplicationDbContext _context;  // Changed from AppDbContext

    public FavoritosRepository(ApplicationDbContext context)  // Changed from AppDbContext
    {
        _context = context;
    }
    public async Task<List<object>> GetFavoritosAsync(string clienteId)
    {
        var favoritos = await _context.ProdutoFavorito
            .Where(f => f.ClienteId == clienteId && f.Efavorito)
            .Include(f => f.Produto)
            .Select(f => new
            {
                f.ProdutoId,
                ProdutoNome = f.Produto.Nome,
                ProdutoPreco = f.Produto.Preco,
                ProdutoDetalhe = f.Produto.Detalhe
            })
            .ToListAsync();

        return favoritos.Cast<object>().ToList(); // Convertendo para List<object>
    }

    public async Task<ProdutoFavorito> GetFavoritoAsync(int produtoId, string clienteId)
    {
        return await _context.ProdutoFavorito
            .FirstOrDefaultAsync(f => f.ProdutoId == produtoId && f.ClienteId == clienteId);
    }

    public async Task AdicionarFavoritoAsync(ProdutoFavorito favorito)
    {
        _context.ProdutoFavorito.Add(favorito);
        await SaveAsync();
    }

    public async Task AtualizarFavoritoAsync(ProdutoFavorito favorito)
    {
        _context.ProdutoFavorito.Update(favorito);
        await SaveAsync();
    }

    public async Task RemoverFavoritoAsync(ProdutoFavorito favorito)
    {
        _context.ProdutoFavorito.Remove(favorito);
        await SaveAsync();
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
