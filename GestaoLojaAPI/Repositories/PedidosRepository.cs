using Microsoft.EntityFrameworkCore;

using GestaoLojaAPI.Context;
using GestaoLojaAPI.Entities;

namespace GestaoLojaAPI.Repositories;
public class PedidosRepository : IPedidosRepository
{
    private readonly AppDbContext _context;

    public PedidosRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Pedidos> GetDetalhesPedidoAsync(int pedidoId)
    {
        return await _context.Pedidos
                             .FirstOrDefaultAsync(p => p.Id == pedidoId);
    }

    public async Task<IEnumerable<Pedidos>> GetPedidosPorUtilizadorAsync(string userId)
    {
        return await _context.Pedidos
                             .Where(p => p.IdUser == userId) // Filtra pedidos pelo utilizador
                             .ToListAsync();
    }

    public async Task<Pedidos> CreatePedidoAsync(Pedidos pedido)
    {
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();
        return pedido;
    }
    public async Task AddPedidosAsync(IEnumerable<Pedidos> pedidos)
    {
        if (pedidos == null || !pedidos.Any())
        {
            throw new ArgumentException("A coleção de pedidos está vazia ou nula.", nameof(pedidos));
        }
        // Adicionar os pedidos à base de dados
        _context.Pedidos.AddRange(pedidos);
        await _context.SaveChangesAsync();
    }
}
