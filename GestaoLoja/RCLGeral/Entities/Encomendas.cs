namespace RCLGeral.Entities;

public class Encomendas
{
    public int Id { get; set; }
    public string? ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string MoradaEntrega { get; set; } = string.Empty;
    public DateTime? DataCriacao { get; set; }
    public string EstadoEncomenda { get; set; } = "Pendente";
    public string EstadoPagamento { get; set; } = "Pendente";
    public decimal ValorTotal { get; set; }
    public ICollection<Pedidos> Pedidos { get; set; } = new List<Pedidos>();
}