using RCLGeral.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace GestaoLojaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UtilizadoresController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public UtilizadoresController(IConfiguration config, UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _config = config;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status201Created)] 
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegistarUser([FromBody] Utilizador utilizador)
    {
        var utilizadorExiste = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == utilizador.Email);

        if (utilizadorExiste is not null)
        {
            return BadRequest(new { success = false, message = "Já existe um utilizador com este email" });
        }

        // Validate TipoRegisto
        if (utilizador.TipoRegisto != "Cliente" && utilizador.TipoRegisto != "Fornecedor")
        {
            return BadRequest(new { success = false, message = "Tipo de registo inválido" });
        }

        var novoUtilizador = new ApplicationUser
        {
            UserName = utilizador.Email,
            Email = utilizador.Email,
            Nome = utilizador.Nome,
            Apelido = utilizador.Apelido,
            NIF = utilizador.NIF,
            Foto = utilizador.Fotografia,
            Localidade = utilizador.Localidade,
            Estado = utilizador.TipoRegisto == "Fornecedor" ? "Pendente" : "Ativo", // Fornecedor needs approval
            StatusFornecedor = utilizador.TipoRegisto == "Fornecedor" ? FornecedorStatus.Pendente : FornecedorStatus.Pendente,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true
        };

        var result = await _userManager.CreateAsync(novoUtilizador, utilizador.Password);
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new { success = false, message = errors });
        }

        // Assign role based on TipoRegisto
        await _userManager.AddToRoleAsync(novoUtilizador, utilizador.TipoRegisto);

        var mensagem = utilizador.TipoRegisto == "Fornecedor"
            ? "Registo de fornecedor efetuado com sucesso! A sua conta está pendente de aprovação pela administração."
            : "Registo efetuado com sucesso! Bem-vindo ao MyMedia.";

        return StatusCode(StatusCodes.Status201Created, new 
        { 
            success = true, 
            message = mensagem
        });
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> LoginUser([FromBody] LoginModel utilizador)
    {
        var utilizadorAtual = await _userManager.Users.FirstOrDefaultAsync(u =>
                                 u.Email == utilizador.Email);

        if (utilizadorAtual is null)
        {
            return NotFound(new { success = false, message = "Utilizador não encontrado" });
        }

        var result = await _signInManager.PasswordSignInAsync(utilizador.Email!, utilizador.Password!, false, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            var tempUser = await _userManager.FindByEmailAsync(utilizador.Email!);
            
            if (utilizadorAtual.Estado != "Ativo")
            {
                return Unauthorized(new { success = false, message = "Utilizador não está ativo." });
            }
            
            var userRoles = await _userManager.GetRolesAsync(tempUser!);
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, utilizadorAtual.Id),
                new Claim(ClaimTypes.Email, utilizador.Email!),
                new Claim(ClaimTypes.Role, userRoles[0]!)
            };

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                success = true,
                token = jwt,
                tokenType = "bearer",
                user = new 
                {
                    id = utilizadorAtual.Id,
                    nome = utilizadorAtual.Nome,
                    apelido = utilizadorAtual.Apelido,
                    email = utilizadorAtual.Email,
                    role = userRoles[0],
                    nif = utilizadorAtual.NIF,
                    rua = utilizadorAtual.Rua,
                    localidade = utilizadorAtual.Localidade,
                    pais = utilizadorAtual.Pais,
                    phoneNumber = utilizadorAtual.PhoneNumber,
                    fotoBase64 = utilizadorAtual.Foto != null ? Convert.ToBase64String(utilizadorAtual.Foto) : null
                }
            });
        }
        else
        {
            return BadRequest(new { success = false, message = "Email ou password incorretos!" });
        }
    }

    [HttpGet("{idUser}")]
    public async Task<IActionResult> GetUserById(string idUser)
    {
        var utilizador = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == idUser);

        if (utilizador is null)
        {
            return NotFound(new { success = false, message = "Utilizador não encontrado" });
        }

        var userRoles = await _userManager.GetRolesAsync(utilizador);

        return Ok(new
        {
            id = utilizador.Id,
            email = utilizador.Email,
            nome = utilizador.Nome,
            apelido = utilizador.Apelido,
            nif = utilizador.NIF,
            rua = utilizador.Rua,
            localidade = utilizador.Localidade,
            pais = utilizador.Pais,
            phoneNumber = utilizador.PhoneNumber,
            role = userRoles.FirstOrDefault(),
            fotoBase64 = utilizador.Foto != null ? Convert.ToBase64String(utilizador.Foto) : null
        });
    }

    [HttpPut("perfil/{userId}")]
    public async Task<IActionResult> AtualizarPerfil(string userId, [FromBody] PerfilUpdateModel model)
    {
        var utilizador = await _userManager.FindByIdAsync(userId);

        if (utilizador is null)
        {
            return NotFound(new { success = false, message = "Utilizador não encontrado" });
        }

        // Update fields
        utilizador.Nome = model.Nome;
        utilizador.Apelido = model.Apelido;
        utilizador.NIF = model.NIF;
        utilizador.Rua = model.Rua;
        utilizador.Localidade = model.Localidade;
        utilizador.Pais = model.Pais;
        utilizador.PhoneNumber = model.PhoneNumber;

        // Update photo if provided
        if (!string.IsNullOrEmpty(model.FotoBase64))
        {
            utilizador.Foto = Convert.FromBase64String(model.FotoBase64);
        }

        var result = await _userManager.UpdateAsync(utilizador);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new { success = false, message = errors });
        }

        return Ok(new { success = true, message = "Perfil atualizado com sucesso" });
    }

    [HttpPost("alterar-password/{userId}")]
    public async Task<IActionResult> AlterarPassword(string userId, [FromBody] AlterarPasswordRequest model)
    {
        var utilizador = await _userManager.FindByIdAsync(userId);

        if (utilizador is null)
        {
            return NotFound(new { success = false, message = "Utilizador não encontrado" });
        }

        var result = await _userManager.ChangePasswordAsync(utilizador, model.PasswordAtual, model.NovaPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new { success = false, message = errors });
        }

        return Ok(new { success = true, message = "Password alterada com sucesso" });
    }
}

// DTOs for the new endpoints
public class PerfilUpdateModel
{
    public string Nome { get; set; } = string.Empty;
    public string Apelido { get; set; } = string.Empty;
    public long? NIF { get; set; }
    public string? Rua { get; set; }
    public string? Localidade { get; set; }
    public string? Pais { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FotoBase64 { get; set; }
}

public class AlterarPasswordRequest
{
    public string PasswordAtual { get; set; } = string.Empty;
    public string NovaPassword { get; set; } = string.Empty;
}
