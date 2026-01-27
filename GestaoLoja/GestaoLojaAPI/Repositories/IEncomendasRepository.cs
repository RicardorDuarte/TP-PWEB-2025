using RCLGeral.Entities;

namespace GestaoLojaAPI.Repositories;

public interface IEncomendasRepository
{
    Task<IEnumerable<Encomendas>> GetAllEncomendasAsync();
    Task<Encomendas> GetEncomendaByIdAsync(int id);
    Task<IEnumerable<Encomendas>> GetEncomendasByUserIdAsync(string clienteID);
    Task<Encomendas> CreateEncomendaAsync(Encomendas encomenda);
    Task<bool> PagarEncomendaAsync(int id, string metodoPagamento);
    Task<bool> CancelarEncomendaAsync(int id);
}
