using RCLGeral.Models;
using System.Net.Http.Json;

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

        public FornecedorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProdutoModel>> GetMeusProdutosAsync()
        {
            try
            {
                var produtos = await _httpClient.GetFromJsonAsync<List<ProdutoModel>>("api/fornecedor/produtos");
                return produtos ?? new List<ProdutoModel>();
            }
            catch (Exception)
            {
                return new List<ProdutoModel>();
            }
        }

        public async Task<ProdutoModel?> GetMeuProdutoAsync(int id)
        {
            try
            {
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
                var response = await _httpClient.PostAsJsonAsync("api/fornecedor/produtos", model);
                
                if (response.IsSuccessStatusCode)
                {
                    var produto = await response.Content.ReadFromJsonAsync<ProdutoModel>();
                    return (true, produto, null);
                }

                var error = await response.Content.ReadAsStringAsync();
                return (false, null, error);
            }
            catch (Exception ex)
            {
                return (false, null, $"Erro de conexão: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? Error)> EditarProdutoAsync(int id, EditarProdutoModel model)
        {
            try
            {
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
                var modos = await _httpClient.GetFromJsonAsync<List<ModoDisponibilizacaoModel>>("api/fornecedor/modos-disponibilizacao");
                return modos ?? new List<ModoDisponibilizacaoModel>();
            }
            catch (Exception)
            {
                return new List<ModoDisponibilizacaoModel>();
            }
        }

        public async Task<List<CategoriaModel>> GetCategoriasAsync()
        {
            try
            {
                var categorias = await _httpClient.GetFromJsonAsync<List<CategoriaModel>>("api/fornecedor/categorias");
                return categorias ?? new List<CategoriaModel>();
            }
            catch (Exception)
            {
                return new List<CategoriaModel>();
            }
        }
    }
}
