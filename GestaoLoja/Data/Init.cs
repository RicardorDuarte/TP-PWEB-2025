using Microsoft.AspNetCore.Identity;

namespace GestaoLoja.Data;

public class Init
{
    public static async Task CriaDadosIniciais(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Adicionar Default Roles
        String[] roles = { "Administrador", "Funcionario", "Cliente", "Fornecedor"};

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Adicionar Default user - Admin
        var defaultUser = new ApplicationUser
        {
            UserName = "admin@isec.pt",
            Email = "admin@isec.pt",
            Nome = "Administrador",
            Apelido = "Local",
            Estado = "Ativo",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
        };

        if (userManager.Users.All(u => u.Email != defaultUser.Email))
        {
            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                var result = await userManager.CreateAsync(defaultUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultUser, "Administrador");
                }
            }
        }
        else
        {
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user != null && !await userManager.IsInRoleAsync(user, "Administrador"))
            {
                await userManager.AddToRoleAsync(user, "Administrador");
            }
        }
    }
}