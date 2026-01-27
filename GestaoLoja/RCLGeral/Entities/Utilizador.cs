namespace RCLGeral.Entities;

public class Utilizador
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Nome { get; set; }
    public string? Apelido { get; set; }
    public long? NIF { get; set; }
    public byte[]? Fotografia { get; set; }
    public string? Localidade { get; set; }
    public string TipoRegisto { get; set; } = "Cliente"; 
}