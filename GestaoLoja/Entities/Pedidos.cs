namespace GestaoLoja.Entities;
public class Pedidos
{
    public int Id { get; set; } // Chave primária

    public int? EncomendaId { get; set; } // Chave estrangeira para a tabela Encomendas
    public string? IdUser { get; set; } // Identifica o utilizador que fez o pedido
    public int ProdutoId { get; set; } // Identifica o produto do pedido

    public int Quantidade { get; set; } // Quantidade do produto

    public decimal PrecoUnitario { get; set; } // Preço unitário do produto

    public decimal ValorTotal { get; set; } // Calculado como PrecoUnitario * Quantidade
    public Encomendas? Encomenda { get; set; } // Relacionamento com Encomenda
}
