using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
#if !ANDROID
using Microsoft.AspNetCore.Http;
#endif

namespace RCLGeral.Entities;

public class Produto
{
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

    [Required]
    [Display(Name = "Preço Base")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecoBase { get; set; }

    [Display(Name = "Percentagem de Lucro (%)")]
    [Column(TypeName = "decimal(5,2)")]
    [Range(0, 100)]
    public decimal PercentagemLucro { get; set; }

    [Display(Name = "Preço Final")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Preco { get; set; }

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = false;

    // Promotion fields
    [Display(Name = "Em Promoção")]
    public bool Promocao { get; set; }

    [Display(Name = "Desconto Promoção (%)")]
    [Column(TypeName = "decimal(5,2)")]
    [Range(0, 100)]
    public decimal PercentagemDesconto { get; set; } = 0;

    [Display(Name = "Preço Promocional")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecoPromocional { get; set; }

    public bool MaisVendido { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Stock { get; set; }
    public bool Disponivel { get; set; }
    public string? Origem { get; set; }

    public string FornecedorId { get; set; } = string.Empty;
    
    [ForeignKey(nameof(FornecedorId))]
    public ApplicationUser? Fornecedor { get; set; }

    public int? CategoriaId { get; set; }
    public Categoria? categoria { get; set; }

    [JsonIgnore]
    public int? ModoEntregaId { get; set; }
    public ModoEntrega? modoentrega { get; set; }

#if !ANDROID
    [NotMapped]
    public IFormFile? ImageFile { get; set; }
#endif

    [NotMapped]
    public decimal PrecoAtual => Promocao && PercentagemDesconto > 0 ? PrecoPromocional : Preco;

    public void CalcularPrecoFinal()
    {
        Preco = PrecoBase + (PrecoBase * (PercentagemLucro / 100));
        
        if (PercentagemDesconto > 0)
        {
            PrecoPromocional = Preco - (Preco * (PercentagemDesconto / 100));
        }
        else
        {
            PrecoPromocional = Preco;
        }
    }
}