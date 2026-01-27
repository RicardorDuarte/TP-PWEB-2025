namespace RCLGeral.Entities;

public class Pedidos
{
    public int Id { get; set; }
    public int? EncomendaId { get; set; }
    public string? IdUser { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public Encomendas? Encomenda { get; set; }
    public Produto? Produto { get; set; }
}