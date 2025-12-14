
namespace GestaoLojaAPI.Entities;
public class ProdutoFavorito
{
    public int Id { get; set; }
    public bool Efavorito { get; set; }
    public int ProdutoId { get; set; }
    public Produto Produto { get; set; }
    public string ClienteId { get; set; }
}
