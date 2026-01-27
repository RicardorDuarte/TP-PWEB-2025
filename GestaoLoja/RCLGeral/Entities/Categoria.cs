using System.ComponentModel.DataAnnotations.Schema;
#if !ANDROID
using Microsoft.AspNetCore.Http;
#endif

namespace RCLGeral.Entities;

public class Categoria
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public int? Ordem { get; set; }
    public string? UrlImagem { get; set; }
    public byte[]? Imagem { get; set; }

#if !ANDROID
    [NotMapped]
    public IFormFile? ImageFile { get; set; }
#endif
}