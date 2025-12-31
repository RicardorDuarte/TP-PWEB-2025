using Microsoft.AspNetCore.Components.Authorization;
using RCLGeral.Services;
using System.Security.Claims;

namespace MyMedia_Web.Components
{
    /// <summary>
    /// Provider de estado de autenticação customizado para Blazor Web
    /// </summary>
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IAuthService _authService;

        public CustomAuthStateProvider(IAuthService authService)
        {
            _authService = authService;
            _authService.OnAuthStateChanged += NotifyAuthStateChanged;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = await _authService.GetUserInfoAsync();
            
            if (user == null)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.NomeCompleto),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, "jwt");
            var principal = new ClaimsPrincipal(identity);

            return new AuthenticationState(principal);
        }

        private void NotifyAuthStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
