using RCLGeral.Models;
using System.Net.Http.Json;

namespace RCLGeral.Services
{
    public interface IEncomendaService
    {
        Task<(bool Success, EncomendaModel? Encomenda, string? Error)> CriarEncomendaAsync(CriarEncomendaModel model);
        Task<List<EncomendaModel>> GetHistoricoAsync();
        Task<EncomendaModel?> GetEncomendaAsync(int id);
        Task<(bool Success, string? Error)> PagarEncomendaAsync(int id, string metodoPagamento);
        Task<(bool Success, string? Error)> CancelarEncomendaAsync(int id);
    }

    public class EncomendaService : IEncomendaService
    {
        private readonly HttpClient _httpClient;

        public EncomendaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool Success, EncomendaModel? Encomenda, string? Error)> CriarEncomendaAsync(CriarEncomendaModel model)
        {
            try
            {
                var requestBody = new
                {
                    Itens = model.Itens.Select(i => new { i.ProdutoId, i.Quantidade }).ToList(),
                    MetodoPagamento = model.MetodoPagamento
                };

                var response = await _httpClient.PostAsJsonAsync("api/encomendas", requestBody);
                
                if (response.IsSuccessStatusCode)
                {
                    var encomenda = await response.Content.ReadFromJsonAsync<EncomendaModel>();
                    return (true, encomenda, null);
                }

                var error = await response.Content.ReadAsStringAsync();
                return (false, null, error);
            }
            catch (Exception ex)
            {
                return (false, null, $"Erro de conexão: {ex.Message}");
            }
        }

        public async Task<List<EncomendaModel>> GetHistoricoAsync()
        {
            try
            {
                var encomendas = await _httpClient.GetFromJsonAsync<List<EncomendaModel>>("api/encomendas/historico");
                return encomendas ?? new List<EncomendaModel>();
            }
            catch (Exception)
            {
                return new List<EncomendaModel>();
            }
        }

        public async Task<EncomendaModel?> GetEncomendaAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<EncomendaModel>($"api/encomendas/{id}");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<(bool Success, string? Error)> PagarEncomendaAsync(int id, string metodoPagamento)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/encomendas/{id}/pagar", 
                    new { MetodoPagamento = metodoPagamento });
                
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

        public async Task<(bool Success, string? Error)> CancelarEncomendaAsync(int id)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/encomendas/{id}/cancelar", null);
                
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
    }
}
