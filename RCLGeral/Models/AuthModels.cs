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
        public string Nome { get; set; } = string.Empty;
        public string Apelido { get; set; } = string.Empty;
        public string Nif { get; set; } = string.Empty;
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

        public string NomeCompleto => $"{Nome} {Apelido}";
        public bool IsCliente => Role == "Cliente";
        public bool IsFornecedor => Role == "Fornecedor";
    }
}
