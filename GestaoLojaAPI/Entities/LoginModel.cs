using System.ComponentModel.DataAnnotations;

namespace GestaoLojaAPI.Entities
{
    public class LoginModel
    {
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Endereço de Email Inválido")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "A password é obrigatória.")]
        public string? Password { get; set; }
    }
}
