using GestaoLojaAPI.Entities;
using RCLAPI.DTO;

namespace GestaoLojaAPI.Repositories;
public interface ICarrinhoRepository
{
    Task<bool> AdicionarOuAtualizarItem(ItemCarrinhoCompra item);
    Task<List<ItemCarrinhoCompra>> ObterCarrinhoPorUser(string userId);
    public Task<bool> RemoverItem(int id);
    public Task<bool> AtualizarItem(ItemCarrinhoCompra item);

}
