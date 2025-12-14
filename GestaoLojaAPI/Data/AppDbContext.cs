using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using GestaoLojaAPI.Data;
using GestaoLojaAPI.Entities;
using RCLAPI.DTO;

namespace GestaoLojaAPI.Context;
public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<ModoEntrega> ModoEntregas { get; set; }
    public DbSet<ProdutoFavorito> ProdutoFavorito { get; set; }
    public DbSet<ItemCarrinhoCompra> ItemCarrinhoCompra { get; set; }
    public DbSet<Encomendas> Encomendas { get; set; }
    public DbSet<Pedidos> Pedidos { get; set; }
}





