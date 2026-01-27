using System.ComponentModel.DataAnnotations;

namespace RCLGeral.Models
{
    public class CheckoutModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O apelido é obrigatório")]
        public string Apelido { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "A morada é obrigatória")]
        public string Morada { get; set; } = string.Empty;

        [Required(ErrorMessage = "O código postal é obrigatório")]
        public string CodigoPostal { get; set; } = string.Empty;

        [Required(ErrorMessage = "A cidade é obrigatória")]
        public string Cidade { get; set; } = string.Empty;

        public string Pais { get; set; } = "Portugal";

        public bool MoradaFaturacaoDiferente { get; set; } = false;
        public string? MoradaFaturacao { get; set; }
        public string? CodigoPostalFaturacao { get; set; }
        public string? CidadeFaturacao { get; set; }
        public string? PaisFaturacao { get; set; }

        public string MetodoPagamento { get; set; } = "MBWay";
        public string? NIF { get; set; }
        public string? Notas { get; set; }

        // Campos para MB WAY
        public string? TelefoneMBWay { get; set; }

        // Campos para Cartão de Crédito
        public string? NumeroCartao { get; set; }
        public string? ValidadeCartao { get; set; }
        public string? CVV { get; set; }
        public string? NomeCartao { get; set; }

        public string MoradaCompleta => $"{Morada}, {CodigoPostal} {Cidade}, {Pais}";
        public string MoradaFaturacaoCompleta => MoradaFaturacaoDiferente 
            ? $"{MoradaFaturacao}, {CodigoPostalFaturacao} {CidadeFaturacao}, {PaisFaturacao}"
            : MoradaCompleta;
    }

    public class FinalizarEncomendaRequest
    {
        public string ClienteId { get; set; } = string.Empty;
        public string MoradaEntrega { get; set; } = string.Empty;
        public string? MoradaFaturacao { get; set; }
        public string? MetodoPagamento { get; set; }
        public string? NIF { get; set; }
        public string? Notas { get; set; }
        public List<PedidoItemRequest> Itens { get; set; } = new();
    }

    public class PedidoItemRequest
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}