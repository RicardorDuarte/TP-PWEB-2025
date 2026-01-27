namespace RCLGeral.Models
{
    public class LoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegistoModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Apelido { get; set; } = string.Empty;
        public long? Nif { get; set; }
        public string Morada { get; set; } = string.Empty;
        public string TipoRegisto { get; set; } = "Cliente"; // Cliente ou Fornecedor
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
        public DateTime? Expiration { get; set; }
        public UserInfo? User { get; set; }
    }

    public class UserInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Apelido { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public long? NIF { get; set; }
        public string? Rua { get; set; }
        public string? Localidade { get; set; }
        public string? Pais { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FotoBase64 { get; set; }

        public string NomeCompleto => $"{Nome} {Apelido}";
        public bool IsCliente => Role == "Cliente";
        public bool IsFornecedor => Role == "Fornecedor";
    }

    public class PerfilModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Apelido { get; set; } = string.Empty;
        public long? NIF { get; set; }
        public string? Rua { get; set; }
        public string? Localidade { get; set; }
        public string? Pais { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FotoBase64 { get; set; }
    }

    public class AlterarPasswordModel
    {
        public string PasswordAtual { get; set; } = string.Empty;
        public string NovaPassword { get; set; } = string.Empty;
        public string ConfirmarPassword { get; set; } = string.Empty;
    }
}
