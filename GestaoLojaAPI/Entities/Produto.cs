using GestaoLojaAPI.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GestaoLojaAPI.Entities;

public class Produto{

    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string? Nome { get; set; }

    [Required]
    [StringLength(200)]
    public string? Detalhe { get; set; }

    [StringLength(200)]
    public string? UrlImagem { get; set; }
    public byte[]? Imagem { get; set; }

    // Preço base definido pelo fornecedor
    [Required]
    [Display(Name = "Preço Base")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecoBase { get; set; }

    // Percentagem de lucro (definida pelo admin/funcionário)
    [Display(Name = "Percentagem de Lucro (%)")]
    [Column(TypeName = "decimal(5,2)")]
    [Range(0, 100)]
    public decimal PercentagemLucro { get; set; }

    // Preço final (preco base + percentagem çucro)
    [Display(Name = "Preço Final")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Preco { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = false;

    public bool Promocao { get; set; }
    public bool MaisVendido { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Stock { get; set; }
    public bool Disponivel { get; set; }
    public string? Origem { get; set; }

    public string FornecedorId { get; set; } = string.Empty;
    [ForeignKey(nameof(FornecedorId))]
    public ApplicationUser? Fornecedor { get; set; }

    public int? CategoriaId { get; set; }
    public Categoria categoria { get; set; }


    [JsonIgnore]
    public int? ModoEntregaId { get; set; }
    public ModoEntrega modoentrega { get; set; }


    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    // Método para calcular o preço final
    public void CalcularPrecoFinal()
    {
        Preco = PrecoBase + (PrecoBase * (PercentagemLucro / 100));
    }

}
