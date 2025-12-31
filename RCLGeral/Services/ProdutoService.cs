using RCLGeral.Models;
using System.Net.Http.Json;

namespace RCLGeral.Services
{
    public interface IProdutoService
    {
        Task<List<ProdutoModel>> GetProdutosAsync(int? categoriaId = null, decimal? precoMin = null, 
            decimal? precoMax = null, string? pesquisa = null, int? modoDisponibilizacaoId = null,
            int pagina = 1, int porPagina = 12);
        Task<ProdutoModel?> GetProdutoAsync(int id);
        Task<ProdutoModel?> GetProdutoDestaqueAsync();
        Task<List<ProdutoModel>> GetProdutosPorCategoriaAsync(int categoriaId);
        Task<List<ProdutoModel>> GetProdutosPromocaoAsync();
        Task<List<ProdutoModel>> GetProdutosMaisVendidosAsync();
    }

    public class ProdutoService : IProdutoService
    {
        private readonly HttpClient _httpClient;

        public ProdutoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProdutoModel>> GetProdutosAsync(int? categoriaId = null, 
            decimal? precoMin = null, decimal? precoMax = null, string? pesquisa = null, 
            int? modoDisponibilizacaoId = null, int pagina = 1, int porPagina = 12)
        {
            try
            {
                // Build query string properly - tipoProduto is required by your API
                var queryParams = new List<string> { "tipoProduto=todos" };
                
                if (categoriaId.HasValue)
                    queryParams.Add($"categoriaId={categoriaId}");
                if (precoMin.HasValue)
                    queryParams.Add($"precoMin={precoMin}");
                if (precoMax.HasValue)
                    queryParams.Add($"precoMax={precoMax}");
                if (!string.IsNullOrEmpty(pesquisa))
                    queryParams.Add($"pesquisa={Uri.EscapeDataString(pesquisa)}");
                if (modoDisponibilizacaoId.HasValue)
                    queryParams.Add($"modoDisponibilizacaoId={modoDisponibilizacaoId}");

                var url = "api/Produtos?" + string.Join("&", queryParams);

                var produtos = await _httpClient.GetFromJsonAsync<List<ProdutoModel>>(url);
                return produtos ?? new List<ProdutoModel>();
            }
            catch (Exception)
            {
                return new List<ProdutoModel>();
            }
        }

        public async Task<ProdutoModel?> GetProdutoAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ProdutoModel>($"api/Produtos/{id}");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ProdutoModel?> GetProdutoDestaqueAsync()
        {
            try
            {
                // Use promocao to get a featured product
                var produtos = await _httpClient.GetFromJsonAsync<List<ProdutoModel>>("api/Produtos?tipoProduto=promocao");
                return produtos?.FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<ProdutoModel>> GetProdutosPorCategoriaAsync(int categoriaId)
        {
            try
            {
                // Fixed: Use the correct API endpoint format
                var produtos = await _httpClient.GetFromJsonAsync<List<ProdutoModel>>(
                    $"api/Produtos?tipoProduto=categoria&categoriaId={categoriaId}");
                return produtos ?? new List<ProdutoModel>();
            }
            catch (Exception)
            {
                return new List<ProdutoModel>();
            }
        }

        public async Task<List<ProdutoModel>> GetProdutosPromocaoAsync()
        {
            try
            {
                var produtos = await _httpClient.GetFromJsonAsync<List<ProdutoModel>>("api/Produtos?tipoProduto=promocao");
                return produtos ?? new List<ProdutoModel>();
            }
            catch (Exception)
            {
                return new List<ProdutoModel>();
            }
        }

        public async Task<List<ProdutoModel>> GetProdutosMaisVendidosAsync()
        {
            try
            {
                var produtos = await _httpClient.GetFromJsonAsync<List<ProdutoModel>>("api/Produtos?tipoProduto=maisvendido");
                return produtos ?? new List<ProdutoModel>();
            }
            catch (Exception)
            {
                return new List<ProdutoModel>();
            }
        }
    }
}
