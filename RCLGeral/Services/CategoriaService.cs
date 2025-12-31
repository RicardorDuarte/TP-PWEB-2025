using RCLGeral.Models;
using System.Net.Http.Json;

namespace RCLGeral.Services
{
    public interface ICategoriaService
    {
        Task<List<CategoriaModel>> GetCategoriasRaizAsync();
        Task<CategoriaModel?> GetCategoriaAsync(int id);
       
    }

    public class CategoriaService : ICategoriaService
    {
        private readonly HttpClient _httpClient;

        public CategoriaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CategoriaModel>> GetCategoriasRaizAsync()
        {
            try
            {
                var categorias = await _httpClient.GetFromJsonAsync<List<CategoriaModel>>("api/Categorias");
                return categorias ?? new List<CategoriaModel>();
            }
            catch (Exception)
            {
                return new List<CategoriaModel>();
            }
        }

        public async Task<CategoriaModel?> GetCategoriaAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CategoriaModel>($"api/Categorias/{id}");
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
