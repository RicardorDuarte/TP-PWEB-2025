using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GestaoLoja.Entities;

public class ModoEntrega{

    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string? Nome { get; set; }

    [StringLength(200)]
    public string? Detalhe { get; set; }


    [JsonIgnore]
    public ICollection<Produto>? Produtos { get; set; }

}
