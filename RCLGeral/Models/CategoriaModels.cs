using System.ComponentModel.DataAnnotations.Schema;

namespace RCLGeral.Models
{
    public class CategoriaModel
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public int? Ordem { get; set; }
        public string? URLImagem { get; set; }
        public byte[]? Imagem { get; set; }
        public string Tipo { get; set; } = string.Empty;  // Added missing property
        public string ImagemUrl
        {
            get
            {
                if (Imagem != null && Imagem.Length > 0)
                    return $"data:image/jpeg;base64,{Convert.ToBase64String(Imagem)}";
                return URLImagem ?? "/images/categoria-placeholder.png";
            }
        }
    }
}
