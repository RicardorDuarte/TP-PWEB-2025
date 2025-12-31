using RCLGeral.Models;

namespace RCLGeral.Services
{
    /// <summary>
    /// Interface para armazenamento de tokens - implementação diferente em Web vs MAUI
    /// </summary>
    public interface ITokenStorageService
    {
        Task<string?> GetTokenAsync();
        Task SetTokenAsync(string token);
        Task RemoveTokenAsync();
        Task<UserInfo?> GetUserAsync();
        Task SetUserAsync(UserInfo user);
        Task RemoveUserAsync();
    }
}
