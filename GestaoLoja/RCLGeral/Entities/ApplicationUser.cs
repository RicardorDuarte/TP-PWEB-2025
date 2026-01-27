#if !ANDROID
using Microsoft.AspNetCore.Identity;
#endif

namespace RCLGeral.Entities;

public class ApplicationUser
#if !ANDROID
    : IdentityUser
#endif
{
#if ANDROID
    // Propriedades base do IdentityUser para Android
    public string Id { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
#endif

    public string? Nome { get; set; }
    public string? Apelido { get; set; }
    public long? NIF { get; set; }
    public string? Rua { get; set; }
    public string? Localidade { get; set; }
    public string? Estado { get; set; }
    public string? Pais { get; set; }
    public string? Role { get; set; }
    public byte[]? Foto { get; set; }
    public FornecedorStatus StatusFornecedor { get; set; } = FornecedorStatus.Pendente;
}