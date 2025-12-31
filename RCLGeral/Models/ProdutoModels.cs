namespace RCLGeral.Models
{
    public class ProdutoModel
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Detalhe { get; set; }
        public string? UrlImagem { get; set; }
        public byte[]? Imagem { get; set; }
        public string? ImagemBase64 { get; set; }
        public decimal PrecoBase { get; set; }
        public decimal Preco { get; set; }
        public decimal Stock { get; set; }
        public bool Ativo { get; set; } = false;
        public bool Promocao { get; set; }
        public bool MaisVendido { get; set; }
        public bool Disponivel { get; set; }
        public string? Origem { get; set; }
        public int? CategoriaId { get; set; }
        public CategoriaModel? categoria { get; set; }
        public int? ModoEntregaId { get; set; }
        public ModoEntregaModel? modoentrega { get; set; }
        public string? FornecedorId { get; set; }
        
        // Properties expected by Razor components
        public string Estado { get; set; } = "Ativo";
        public string ModoDisponibilizacao => modoentrega?.Nome ?? "Venda";
        public List<string> Categorias => categoria != null ? new List<string> { categoria.Nome ?? "" } : new List<string>();
        public string FornecedorNome { get; set; } = string.Empty;

        // Computed properties
        public bool TemStock => Stock > 0;
        public bool EstaAtivo => Ativo;
        public bool PodeComprar => TemStock && EstaAtivo;

        public string ImagemUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(ImagemBase64))
                    return $"data:image/jpeg;base64,{ImagemBase64}";
                if (Imagem != null && Imagem.Length > 0)
                    return $"data:image/jpeg;base64,{Convert.ToBase64String(Imagem)}";
                return UrlImagem ?? "/images/produto-placeholder.png";
            }
        }
    }

    public class CriarProdutoModel
    {
        public string Nome { get; set; } = string.Empty;
        public string Detalhe { get; set; } = string.Empty;
        public string? UrlImagem { get; set; }
        public string? ImagemBase64 { get; set; }
        public decimal PrecoBase { get; set; }
        public decimal Stock { get; set; }
        public int ModoDisponibilizacaoId { get; set; }
        public List<int> CategoriaIds { get; set; } = new();
    }

    public class EditarProdutoModel : CriarProdutoModel
    {
        public int Id { get; set; }
    }

    public class ModoDisponibilizacaoModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Detalhe { get; set; }
        public string Tipo { get; set; } = string.Empty;
    }

    public class ModoEntregaModel
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Detalhe { get; set; }
    }
}
