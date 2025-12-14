using GestaoLojaAPI.Entities;
using RCLAPI.DTO;

namespace GestaoLojaAPI.Repositories;
public interface IPedidosRepository
{
    Task<Pedidos> GetDetalhesPedidoAsync(int pedidoId);
    Task<IEnumerable<Pedidos>> GetPedidosPorUtilizadorAsync(string userId);
    Task<Pedidos> CreatePedidoAsync(Pedidos pedido);
    Task AddPedidosAsync(IEnumerable<Pedidos> pedidos);
}
