using Microsoft.EntityFrameworkCore;
using GestaoLoja.Data;       // Changed from GestaoLojaAPI.Context
using RCLGeral.Entities;   // Changed from GestaoLojaAPI.Entities

namespace GestaoLojaAPI.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly ApplicationDbContext _context;  // Changed from AppDbContext

    public CategoriaRepository(ApplicationDbContext context)  // Changed from AppDbContext
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Categoria>> GetCategorias()
    {
        return await _context.Categorias.ToListAsync();
    }
}
    