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
        private readonly IAuthService _authService;

        public EncomendaService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
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
                var user = await _authService.GetUserInfoAsync();
                if (user == null)
                {
                    return new List<EncomendaModel>();
                }

                var encomendas = await _httpClient.GetFromJsonAsync<List<EncomendaApiModel>>(
                    $"api/Encomendas/cliente/{user.Id}");

                if (encomendas == null)
                {
                    return new List<EncomendaModel>();
                }

                return encomendas.Select(e => new EncomendaModel
                {
                    Id = e.Id,
                    DataCriacao = e.DataCriacao ?? DateTime.Now,
                    Estado = e.EstadoEncomenda,
                    EstadoPagamento = e.EstadoPagamento,
                    Total = e.ValorTotal,
                    Itens = e.Pedidos?.Select(p => new EncomendaItemModel
                    {
                        Id = p.Id,
                        ProdutoId = p.ProdutoId,
                        ProdutoNome = p.Produto?.Nome ?? "Produto",
                        ProdutoImagem = p.Produto?.GetImagemUrl(),
                        Quantidade = p.Quantidade,
                        PrecoUnitario = p.PrecoUnitario
                    }).ToList() ?? new List<EncomendaItemModel>()
                }).ToList();
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
                var encomenda = await _httpClient.GetFromJsonAsync<EncomendaApiModel>(
                    $"api/Encomendas/{id}");

                if (encomenda == null)
                {
                    return null;
                }

                return new EncomendaModel
                {
                    Id = encomenda.Id,
                    DataCriacao = encomenda.DataCriacao ?? DateTime.Now,
                    Estado = encomenda.EstadoEncomenda,
                    EstadoPagamento = encomenda.EstadoPagamento,
                    Total = encomenda.ValorTotal,
                    Itens = encomenda.Pedidos?.Select(p => new EncomendaItemModel
                    {
                        Id = p.Id,
                        ProdutoId = p.ProdutoId,
                        ProdutoNome = p.Produto?.Nome ?? "Produto",
                        ProdutoImagem = p.Produto?.GetImagemUrl(),
                        Quantidade = p.Quantidade,
                        PrecoUnitario = p.PrecoUnitario
                    }).ToList() ?? new List<EncomendaItemModel>()
                };
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

        internal class EncomendaApiModel
        {
            public int Id { get; set; }
            public string? ClienteId { get; set; }
            public string ClienteNome { get; set; } = string.Empty;
            public string MoradaEntrega { get; set; } = string.Empty;
            public DateTime? DataCriacao { get; set; }
            public string EstadoEncomenda { get; set; } = "Pendente";
            public string EstadoPagamento { get; set; } = "Pendente";
            public decimal ValorTotal { get; set; }
            public List<PedidoApiModel>? Pedidos { get; set; }
        }

        internal class PedidoApiModel
        {
            public int Id { get; set; }
            public int ProdutoId { get; set; }
            public int Quantidade { get; set; }
            public decimal PrecoUnitario { get; set; }
            public decimal ValorTotal { get; set; }
            public ProdutoApiModel? Produto { get; set; }
        }

        internal class ProdutoApiModel
        {
            public int Id { get; set; }
            public string? Nome { get; set; }
            public string? UrlImagem { get; set; }
            public byte[]? Imagem { get; set; }
            
            public string? GetImagemUrl()
            {
                if (!string.IsNullOrEmpty(UrlImagem))
                    return UrlImagem;
                if (Imagem != null && Imagem.Length > 0)
                    return $"data:image/jpeg;base64,{Convert.ToBase64String(Imagem)}";
                return null;
            }
        }
    }
}
