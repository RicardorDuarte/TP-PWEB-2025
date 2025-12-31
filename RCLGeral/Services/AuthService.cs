using RCLGeral.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace RCLGeral.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginModel model);
        Task<AuthResponse> RegistarAsync(RegistoModel model);
        Task LogoutAsync();
        Task<UserInfo?> GetUserInfoAsync();
        Task<bool> IsAuthenticatedAsync();
        string? GetToken();
        event Action? OnAuthStateChanged;
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenStorage;
        private string? _cachedToken;
        private UserInfo? _cachedUser;

        public event Action? OnAuthStateChanged;

        public AuthService(HttpClient httpClient, ITokenStorageService tokenStorage)
        {
            _httpClient = httpClient;
            _tokenStorage = tokenStorage;
        }

        public async Task<AuthResponse> LoginAsync(LoginModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Utilizadores/LoginUser", model);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    if (result?.Success == true && result.Token != null)
                    {
                        await _tokenStorage.SetTokenAsync(result.Token);
                        await _tokenStorage.SetUserAsync(result.User!);
                        _cachedToken = result.Token;
                        _cachedUser = result.User;
                        ConfigureHttpClient(result.Token);
                        OnAuthStateChanged?.Invoke();
                    }
                    return result ?? new AuthResponse { Success = false, Message = "Erro ao processar resposta" };
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = response.StatusCode == System.Net.HttpStatusCode.Unauthorized 
                        ? "Email ou password incorretos" 
                        : $"Erro: {response.StatusCode}" 
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = $"Erro de conexão: {ex.Message}" };
            }
        }

        public async Task<AuthResponse> RegistarAsync(RegistoModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Utilizadores/RegistarUser", model);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    return result ?? new AuthResponse { Success = false, Message = "Erro ao processar resposta" };
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = response.StatusCode == System.Net.HttpStatusCode.BadRequest
                        ? errorContent
                        : $"Erro: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = $"Erro de conexão: {ex.Message}" };
            }
        }

        public async Task LogoutAsync()
        {
            await _tokenStorage.RemoveTokenAsync();
            await _tokenStorage.RemoveUserAsync();
            _cachedToken = null;
            _cachedUser = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
            OnAuthStateChanged?.Invoke();
        }

        public async Task<UserInfo?> GetUserInfoAsync()
        {
            if (_cachedUser != null)
                return _cachedUser;

            _cachedUser = await _tokenStorage.GetUserAsync();
            return _cachedUser;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public string? GetToken() => _cachedToken;

        private async Task<string?> GetTokenAsync()
        {
            if (_cachedToken != null)
                return _cachedToken;

            _cachedToken = await _tokenStorage.GetTokenAsync();
            if (!string.IsNullOrEmpty(_cachedToken))
            {
                ConfigureHttpClient(_cachedToken);
            }
            return _cachedToken;
        }

        private void ConfigureHttpClient(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task InitializeAsync()
        {
            var token = await _tokenStorage.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _cachedToken = token;
                _cachedUser = await _tokenStorage.GetUserAsync();
                ConfigureHttpClient(token);
            }
        }
    }
}
