using RCLGeral.Entities;

namespace GestaoLojaAPI.Repositories;

public interface IFavoritosRepository
{
    Task<List<object>> GetFavoritosAsync(string clienteId);
    Task<ProdutoFavorito> GetFavoritoAsync(int produtoId, string clienteId);
    Task AdicionarFavoritoAsync(ProdutoFavorito favorito);
    Task AtualizarFavoritoAsync(ProdutoFavorito favorito);
    Task RemoverFavoritoAsync(ProdutoFavorito favorito);
    Task SaveAsync();
}
