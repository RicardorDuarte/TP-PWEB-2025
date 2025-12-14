using GestaoLoja.Entities;

public class Encomendas
{
    public int Id { get; set; } // Chave primária

    public string? ClienteId { get; set; } // Identifica o cliente que fez a encomenda
    public string MoradaEntrega { get; set; } // Morada de entrega da encomenda
    public DateTime? DataCriacao { get; set; } // Data de criação da encomen
    public string EstadoEncomenda { get; set; } = "Pendente"; 
    public string EstadoPagamento { get; set; } = "Pendente"; 
    public decimal ValorTotal { get; set; } // Soma do valor de todos os itens desta encomenda
    public ICollection<Pedidos> Pedidos { get; set; } = new List<Pedidos>(); // Relacionamento com os pedidos
}