using RCLGeral.Entities;

namespace GestaoLojaAPI.Repositories;

public interface ICategoriaRepository
{
    Task<IEnumerable<Categoria>> GetCategorias();
}
