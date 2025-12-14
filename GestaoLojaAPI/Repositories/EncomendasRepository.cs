using Microsoft.EntityFrameworkCore;

using GestaoLojaAPI.Context;
using GestaoLojaAPI.Entities;

namespace GestaoLojaAPI.Repositories
{
    public class EncomendasRepository : IEncomendasRepository
    {
        private readonly AppDbContext _context;

        public EncomendasRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Encomendas>> GetAllEncomendasAsync()
        {
            return await _context.Set<Encomendas>().ToListAsync(); // pode incluir qualquer filtragem ou ordenação
        }

        public async Task<Encomendas> GetEncomendaByIdAsync(int id)
        {
            return await _context.Set<Encomendas>().FindAsync(id); // Encontrar a encomenda pelo ID
        }

        public async Task<Encomendas> CreateEncomendaAsync(Encomendas encomenda)
        {
                // Adicionar a encomenda à base de dados
                _context.Encomendas.Add(encomenda);
                await _context.SaveChangesAsync();

                // Retornar a encomenda com o ID gerado
                return encomenda;
        }
       
    }
}
