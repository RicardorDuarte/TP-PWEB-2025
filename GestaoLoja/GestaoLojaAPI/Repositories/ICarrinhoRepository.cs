using RCLGeral.Entities;

namespace GestaoLojaAPI.Repositories;

public interface ICarrinhoRepository
{
    Task<bool> AdicionarOuAtualizarItem(ItemCarrinhoCompra item);
    Task<List<ItemCarrinhoCompra>> ObterCarrinhoPorUser(string userId);
    Task<bool> RemoverItem(int id);
    Task<bool> AtualizarItem(ItemCarrinhoCompra item);
}
