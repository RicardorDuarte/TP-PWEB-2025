using Microsoft.EntityFrameworkCore;

using GestaoLojaAPI.Context;
using GestaoLojaAPI.Entities;

namespace GestaoLojaAPI.Repositories;
public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext dbContext;
    public CategoriaRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<IEnumerable<Categoria>> GetCategorias()
    {
        return await dbContext.Categorias.ToListAsync();
    }
}
