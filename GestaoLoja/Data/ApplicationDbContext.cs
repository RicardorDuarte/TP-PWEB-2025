using GestaoLoja.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestaoLoja.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options){
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<ModoEntrega> ModoEntregas { get; set; }
    public DbSet<ProdutoFavorito> ProdutoFavorito { get; set; }
    public DbSet<ItemCarrinhoCompra> ItemCarrinhoCompra { get; set; }
    public DbSet<Pedidos> Pedidos { get; set; }
    public DbSet<Encomendas> Encomendas { get; set; }
}
