using RCLGeral.Models;
using RCLGeral.Services;
using Microsoft.JSInterop;
using System.Text.Json;

namespace MyMedia_Web.Components
{
    /// <summary>
    /// Implementação de armazenamento de tokens para Blazor Web usando localStorage
    /// </summary>
    public class WebTokenStorageService : ITokenStorageService
    {
        private readonly IJSRuntime _jsRuntime;
        private const string TokenKey = "mycoll_auth_token";
        private const string UserKey = "mycoll_user_info";

        public WebTokenStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);
            }
            catch
            {
                return null;
            }
        }

        public async Task SetTokenAsync(string token)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        }

        public async Task RemoveTokenAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        }

        public async Task<UserInfo?> GetUserAsync()
        {
            try
            {
                var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", UserKey);
                if (string.IsNullOrEmpty(json))
                    return null;

                return JsonSerializer.Deserialize<UserInfo>(json);
            }
            catch
            {
                return null;
            }
        }

        public async Task SetUserAsync(UserInfo user)
        {
            var json = JsonSerializer.Serialize(user);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserKey, json);
        }

        public async Task RemoveUserAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", UserKey);
        }
    }
}
