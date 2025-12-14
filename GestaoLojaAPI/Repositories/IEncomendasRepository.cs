using GestaoLojaAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoLojaAPI.Repositories
{
    public interface IEncomendasRepository
    {
        Task<IEnumerable<Encomendas>> GetAllEncomendasAsync();
        Task<Encomendas> GetEncomendaByIdAsync(int id);
        Task<Encomendas> CreateEncomendaAsync(Encomendas encomenda);  // Novo método para criar encomenda
    }
}
