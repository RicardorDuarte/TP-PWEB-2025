using GestaoLojaAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoLojaAPI.Repositories
{
    public interface IFavoritosRepository
    {
        Task<List<dynamic>> GetFavoritosAsync(string clienteId);
        Task<ProdutoFavorito> GetFavoritoAsync(int produtoId, string clienteId);
        Task AdicionarFavoritoAsync(ProdutoFavorito favorito);
        Task AtualizarFavoritoAsync(ProdutoFavorito favorito);
        Task RemoverFavoritoAsync(ProdutoFavorito favorito);
        Task SaveAsync();
    }
}
