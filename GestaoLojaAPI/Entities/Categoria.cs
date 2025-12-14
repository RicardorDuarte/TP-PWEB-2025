using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoLojaAPI.Entities;

public class Categoria{

    public int Id { get; set; }
    public string? Nome { get; set; }

    public int? Ordem { get; set; }

    public string? URLImagem { get; set; }
    public byte[]? Imagem { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; }

}
