using Microsoft.AspNetCore.Identity;
using GestaoLoja.Entities;

namespace GestaoLoja.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser{

    public string? Nome { get; set; }
    public string? Apelido { get; set; }
    public long? NIF { get; set; }
    public string? Rua { get; set; }
    public string? Localidade { get; set; }
    public string? Estado { get; set; }
    public string? Pais { get; set; }
    public string? Role { get; set; }
    public Byte[]? Foto { get; set; }

    public FornecedorStatus StatusFornecedor { get; set; } = FornecedorStatus.Pendente;

}
