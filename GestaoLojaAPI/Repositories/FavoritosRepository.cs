using GestaoLojaAPI.Context;
using GestaoLojaAPI.Data;
using GestaoLojaAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoLojaAPI.Repositories
{
    public class FavoritosRepository : IFavoritosRepository
    {
        private readonly AppDbContext _context;

        public FavoritosRepository(AppDbContext context)
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
}
