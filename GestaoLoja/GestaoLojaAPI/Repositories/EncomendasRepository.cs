using Microsoft.EntityFrameworkCore;
using GestaoLoja.Data;
using RCLGeral.Entities;

namespace GestaoLojaAPI.Repositories;

public class EncomendasRepository : IEncomendasRepository
{
    private readonly ApplicationDbContext _context;

    public EncomendasRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Encomendas>> GetAllEncomendasAsync()
    {
        return await _context.Encomendas
            .Include(e => e.Pedidos)
            .OrderByDescending(e => e.DataCriacao)
            .ToListAsync();
    }

    public async Task<Encomendas?> GetEncomendaByIdAsync(int id)
    {
        return await _context.Encomendas
            .Include(e => e.Pedidos)
                .ThenInclude(p => p.Produto)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Encomendas>> GetEncomendasByUserIdAsync(string clienteId)
    {
        return await _context.Encomendas
            .Include(e => e.Pedidos)
                .ThenInclude(p => p.Produto)
            .Where(e => e.ClienteId == clienteId)
            .OrderByDescending(e => e.DataCriacao)
            .ToListAsync();
    }

    public async Task<Encomendas> CreateEncomendaAsync(Encomendas encomenda)
    {
            // Adicionar a encomenda à base de dados
            _context.Encomendas.Add(encomenda);
            await _context.SaveChangesAsync();

            // Retornar a encomenda com o ID gerado
            return encomenda;
        
       
    }

    public async Task<bool> PagarEncomendaAsync(int id, string metodoPagamento)
    {
        var encomenda = await _context.Encomendas.FindAsync(id);
        if (encomenda == null)
            return false;

        // Atualizar o estado de pagamento para "Pago" e guardar o método
        encomenda.EstadoPagamento = "Pago";
        encomenda.EstadoEncomenda = "Paga";
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelarEncomendaAsync(int id)
    {
        var encomenda = await _context.Encomendas.FindAsync(id);
        if (encomenda == null)
            return false;

        // Só pode cancelar se ainda não foi expedida
        if (encomenda.EstadoEncomenda == "Expedido" || encomenda.EstadoEncomenda == "Concluida")
            return false;

        encomenda.EstadoEncomenda = "Cancelada";
        encomenda.EstadoPagamento = "Cancelado";
        
        await _context.SaveChangesAsync();
        return true;
    }
}
