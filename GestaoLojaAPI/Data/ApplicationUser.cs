using Microsoft.AspNetCore.Identity;

namespace GestaoLojaAPI.Data;

public class ApplicationUser : IdentityUser
{
    public string? Nome { get; set; }
    public string? Apelido { get; set; }
    public long? NIF { get; set; }
    public string? Rua { get; set; }
    public string? Estado { get; set; }
    public string? Localidade { get; set; }
    public string? Pais { get; set; }
    public Byte[]? Foto { get; set; }
}
