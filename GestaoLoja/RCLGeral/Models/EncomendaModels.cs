namespace RCLGeral.Models
{
    public class EncomendaModel
    {
        public int Id { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataPagamento { get; set; }
        public DateTime? DataExpedicao { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string EstadoPagamento { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string? MetodoPagamento { get; set; }
        public string? ReferenciaPagamento { get; set; }
        public List<EncomendaItemModel> Itens { get; set; } = new();

        // Propriedades calculadas para UI
        // Só pode pagar se o estado de pagamento é "Aguarda Pagamento" (escolheu pagar depois)
        public bool PodePagar => EstadoPagamento == "Aguarda Pagamento";
        // Só pode cancelar antes de ser expedida e antes de pagar
        public bool PodeCancelar => (Estado == "Pendente" || Estado == "Confirmada") && EstadoPagamento != "Pago";
        public int TotalItens => Itens.Sum(i => i.Quantidade);
    }

    public class EncomendaItemModel
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; } = string.Empty;
        public string? ProdutoImagem { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal => Quantidade * PrecoUnitario;
    }

    public class CriarEncomendaModel
    {
        public List<ItemCarrinhoModel> Itens { get; set; } = new();
        public string? MetodoPagamento { get; set; }
    }

    public class ItemCarrinhoModel
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }
}
