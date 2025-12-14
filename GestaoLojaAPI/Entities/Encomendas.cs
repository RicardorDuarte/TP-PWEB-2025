namespace GestaoLojaAPI.Entities
{
    public class Encomendas
    {
        public int Id { get; set; } // Chave primária

        public string ClienteId { get; set; } // Identifica o cliente que fez a encomenda
        public string MoradaEntrega { get; set; } // Morada de entrega da encomenda
        public DateTime? DataCriacao { get; set; } // Data de criação da encomenda

        public string EstadoEncomenda { get; set; } = "Pendente"; // Exemplo: "Pendente", "Concluída", "Cancelada"

        public string EstadoPagamento { get; set; } = "Não Pago"; // Exemplo: "Pago", "Não Pago"

        public decimal ValorTotal { get; set; } // Soma do valor de todos os itens desta encomenda

        public ICollection<Pedidos> Pedidos { get; set; } = new List<Pedidos>(); // Relacionamento com os pedidos
    }
}
