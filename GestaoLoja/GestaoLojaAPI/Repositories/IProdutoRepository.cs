using RCLGeral.Entities;

namespace GestaoLojaAPI.Repositories;

public interface IProdutoRepository
{
    Task<IEnumerable<Produto>> ObterProdutosPorCategoriaAsync(int categoriaId);
    Task<IEnumerable<Produto>> ObterProdutosPromocaoAsync();
    Task<IEnumerable<Produto>> ObterProdutosMaisVendidosAsync();
    Task<IEnumerable<Produto>> ObterTodosProdutosAsync();
    Task<Produto> ObterDetalheProdutoAsync(int id);
    Task<Produto?> GetProdutoByIdAsync(int id);
}
