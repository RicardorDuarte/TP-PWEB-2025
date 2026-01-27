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
        Task<UserInfo?> GetCurrentUserAsync();
        Task<bool> IsAuthenticatedAsync();
        string? GetToken();
        Task<UserInfo?> GetPerfilAsync();
        Task<AuthResponse> AtualizarPerfilAsync(PerfilModel model);
        Task<AuthResponse> AlterarPasswordAsync(AlterarPasswordModel model);
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
                
                // Mensagens de erro mais claras
                string mensagemErro = response.StatusCode switch
                {
                    System.Net.HttpStatusCode.Unauthorized => "Email ou palavra-passe incorretos",
                    System.Net.HttpStatusCode.BadRequest => "Email ou palavra-passe incorretos. Verifique os dados introduzidos.",
                    System.Net.HttpStatusCode.NotFound => "Utilizador não encontrado",
                    System.Net.HttpStatusCode.Forbidden => "Conta suspensa ou sem permissões de acesso",
                    _ => "Erro ao fazer login. Tente novamente mais tarde."
                };
                
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = mensagemErro 
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

        public async Task<UserInfo?> GetCurrentUserAsync()
        {
            return await GetUserInfoAsync();
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public string? GetToken() => _cachedToken;

        public async Task<UserInfo?> GetPerfilAsync()
        {
            try
            {
                var user = await GetUserInfoAsync();
                if (user == null) return null;

                var response = await _httpClient.GetFromJsonAsync<UserInfo>($"api/Utilizadores/{user.Id}");
                return response;
            }
            catch
            {
                return null;
            }
        }

        public async Task<AuthResponse> AtualizarPerfilAsync(PerfilModel model)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Utilizadores/perfil/{model.Id}", new
                {
                    nome = model.Nome,
                    apelido = model.Apelido,
                    nif = model.NIF,
                    rua = model.Rua,
                    localidade = model.Localidade,
                    pais = model.Pais,
                    phoneNumber = model.PhoneNumber,
                    fotoBase64 = model.FotoBase64
                });

                if (response.IsSuccessStatusCode)
                {
                    // Update cached user
                    if (_cachedUser != null)
                    {
                        _cachedUser.Nome = model.Nome;
                        _cachedUser.Apelido = model.Apelido;
                        _cachedUser.NIF = model.NIF;
                        _cachedUser.Rua = model.Rua;
                        _cachedUser.Localidade = model.Localidade;
                        _cachedUser.Pais = model.Pais;
                        _cachedUser.PhoneNumber = model.PhoneNumber;
                        _cachedUser.FotoBase64 = model.FotoBase64;
                        await _tokenStorage.SetUserAsync(_cachedUser);
                    }
                    OnAuthStateChanged?.Invoke();
                    return new AuthResponse { Success = true, Message = "Perfil atualizado com sucesso" };
                }

                return new AuthResponse { Success = false, Message = "Erro ao atualizar perfil" };
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = $"Erro: {ex.Message}" };
            }
        }

        public async Task<AuthResponse> AlterarPasswordAsync(AlterarPasswordModel model)
        {
            try
            {
                var user = await GetUserInfoAsync();
                if (user == null)
                    return new AuthResponse { Success = false, Message = "Utilizador não autenticado" };

                var response = await _httpClient.PostAsJsonAsync($"api/Utilizadores/alterar-password/{user.Id}", new
                {
                    passwordAtual = model.PasswordAtual,
                    novaPassword = model.NovaPassword
                });

                if (response.IsSuccessStatusCode)
                {
                    return new AuthResponse { Success = true, Message = "Password alterada com sucesso" };
                }

                var error = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return error ?? new AuthResponse { Success = false, Message = "Erro ao alterar password" };
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = $"Erro: {ex.Message}" };
            }
        }

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
