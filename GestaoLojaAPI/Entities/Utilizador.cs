using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoLojaAPI.Entities
{
    public class Utilizador
    {
        public string? Nome { get; set; }
        public string? Apelido { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Endereço de Email Inválido")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "A indicação da Password é obrigatória!")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A Password deve ter entre 6 e 100 caracteres.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{6,}$",
            ErrorMessage = "A senha deve conter pelo menos uma letra maiúscula, um número e um caractere especial.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "A confirmação da Password é obrigatória!")]
        [Compare("Password", ErrorMessage = "A Password e a Confirmação da Password não coincidem")]
        public string ConfirmPassword { get; set; }


        [ValidarNIF(ErrorMessage = "NIF inválido!")]
        public long? NIF { get; set; }
        public string? Rua { get; set; }
        public string? Estado { get; set; }
        public string? Localidade { get; set; }
        public string? Pais { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public byte[]? Fotografia { get; set; }
        public string? UrlImagem { get; set; }

        // Validação customizada para o NIF
        public class ValidarNIF : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                // Inserir o código que está no site das Finanças
                if (value is long nif)
                {
                    return nif > 100;
                }
                return false;
            }
        }
    }
}
