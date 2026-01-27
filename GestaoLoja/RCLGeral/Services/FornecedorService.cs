using RCLGeral.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace RCLGeral.Services
{
    public interface IFornecedorService
    {
        Task<List<ProdutoModel>> GetMeusProdutosAsync();
        Task<ProdutoModel?> GetMeuProdutoAsync(int id);
        Task<(bool Success, ProdutoModel? Produto, string? Error)> CriarProdutoAsync(CriarProdutoModel model);
        Task<(bool Success, string? Error)> EditarProdutoAsync(int id, EditarProdutoModel model);
        Task<(bool Success, string? Error)> ApagarProdutoAsync(int id);
        Task<(bool Success, string? Error)> SuspenderProdutoAsync(int id);
        Task<(bool Success, string? Error)> ReativarProdutoAsync(int id);
        Task<List<EncomendaModel>> GetHistoricoVendasAsync();
        Task<List<ModoDisponibilizacaoModel>> GetModosDisponibilizacaoAsync();
        Task<List<CategoriaModel>> GetCategoriasAsync();
    }

    public class FornecedorService : IFornecedorService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenStorage;

        public FornecedorService(HttpClient httpClient, ITokenStorageService tokenStorage)
        {
            _httpClient = httpClient;
            _tokenStorage = tokenStorage;
        }

        private async Task ConfigureAuthHeaderAsync()
        {
            var token = await _tokenStorage.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<ProdutoModel>> GetMeusProdutosAsync()
        {
            try
            {
                await ConfigureAuthHeaderAsync();
                
                Console.WriteLine($"GetMeusProdutosAsync - Auth: {_httpClient.DefaultRequestHeaders.Authorization}");
                
                var response = await _httpClient.GetAsync("api/fornecedor/produtos");
                
                Console.WriteLine($"GetMeusProdutosAsync - Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var produtos = await response.Content.ReadFromJsonAsync<List<ProdutoModel>>();
                    Console.WriteLine($"GetMeusProdutosAsync - Produtos: {produtos?.Count ?? 0}");
                    return produtos ?? new List<ProdutoModel>();
                }
                
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"GetMeusProdutosAsync - Error: {error}");
                return new List<ProdutoModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetMeusProdutosAsync - Exception: {ex.Message}");
                return new List<ProdutoModel>();
            }
        }

        public async Task<ProdutoModel?> GetMeuProdutoAsync(int id)
        {
            try
            {
                await ConfigureAuthHeaderAsync();
                return await _httpClient.GetFromJsonAsync<ProdutoModel>($"api/fornecedor/produtos/{id}");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<(bool Success, ProdutoModel? Produto, string? Error)> CriarProdutoAsync(CriarProdutoModel model)
        {
            try
            {
                await ConfigureAuthHeaderAsync();
                
                Console.WriteLine($"Sending request to api/fornecedor/produtos");
                Console.WriteLine($"Auth header: {_httpClient.DefaultRequestHeaders.Authorization}");
                
                var response = await _httpClient.PostAsJsonAsync("api/fornecedor/produtos", model);
                
                Console.WriteLine($"Response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var produto = await response.Content.ReadFromJsonAsync<ProdutoModel>();
                    return (true, produto, null);
                }

                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error response: {error}");
                
                // Return more descriptive error
                return (false, null, $"Status: {(int)response.StatusCode} {response.StatusCode}. {error}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                return (false, null, $"Erro de conexão: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? Error)> EditarProdutoAsync(int id, EditarProdutoModel model)
        {
            try
            {
                await ConfigureAuthHeaderAsync();
                var response = await _httpClient.PutAsJsonAsync($"api/fornecedor/produtos/{id}", model);
                
                if (response.IsSuccessStatusCode)
                    return (true, null);

                var error = await response.Content.ReadAsStringAsync();
                return (false, error);
            }
            catch (Exception ex)
            {
                return (false, $"Erro de conexão: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? Error)> ApagarProdutoAsync(int id)
        {
            try
            {
                await ConfigureAuthHeaderAsync();
                var response = await _httpClient.DeleteAsync($"api/fornecedor/produtos/{id}");
                
                if (response.IsSuccessStatusCode)
                    return (true, null);

                var error = await response.Content.ReadAsStringAsync();
                return (false, error);
            }
            catch (Exception ex)
            {
                return (false, $"Erro de conexão: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? Error)> SuspenderProdutoAsync(int id)
        {
            try
            {
                await ConfigureAuthHeaderAsync();
                var response = await _httpClient.PostAsync($"api/fornecedor/produtos/{id}/suspender", null);
                
                if (response.IsSuccessStatusCode)
                    return (true, null);

                var error = await response.Content.ReadAsStringAsync();
                return (false, error);
            }
            catch (Exception ex)
            {
                return (false, $"Erro de conexão: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? Error)> ReativarProdutoAsync(int id)
        {
            try
            {
                await ConfigureAuthHeaderAsync();
                var response = await _httpClient.PostAsync($"api/fornecedor/produtos/{id}/reativar", null);
                
                if (response.IsSuccessStatusCode)
                    return (true, null);

                var error = await response.Content.ReadAsStringAsync();
                return (false, error);
            }
            catch (Exception ex)
            {
                return (false, $"Erro de conexão: {ex.Message}");
            }
        }

        public async Task<List<EncomendaModel>> GetHistoricoVendasAsync()
        {
            try
            {
                await ConfigureAuthHeaderAsync();
                var vendas = await _httpClient.GetFromJsonAsync<List<EncomendaModel>>("api/fornecedor/vendas");
                return vendas ?? new List<EncomendaModel>();
            }
            catch (Exception)
            {
                return new List<EncomendaModel>();
            }
        }

        public async Task<List<ModoDisponibilizacaoModel>> GetModosDisponibilizacaoAsync()
        {
            try
            {
                // No auth needed for this endpoint
                var modos = await _httpClient.GetFromJsonAsync<List<ModoDisponibilizacaoModel>>("api/fornecedor/modos-disponibilizacao");
                return modos ?? new List<ModoDisponibilizacaoModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetModosDisponibilizacaoAsync error: {ex.Message}");
                return new List<ModoDisponibilizacaoModel>();
            }
        }

        public async Task<List<CategoriaModel>> GetCategoriasAsync()
        {
            try
            {
                // No auth needed for this endpoint
                var categorias = await _httpClient.GetFromJsonAsync<List<CategoriaModel>>("api/fornecedor/categorias");
                return categorias ?? new List<CategoriaModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetCategoriasAsync error: {ex.Message}");
                return new List<CategoriaModel>();
            }
        }
    }
}
