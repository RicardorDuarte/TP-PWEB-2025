using GestaoLojaAPI.Entities;

namespace GestaoLojaAPI.Repositories;
public interface ICategoriaRepository
{
    Task<IEnumerable<Categoria>> GetCategorias();
}
