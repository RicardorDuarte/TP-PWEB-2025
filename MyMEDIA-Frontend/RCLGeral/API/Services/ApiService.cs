using Microsoft.Extensions.Logging;
using SharedEntities.BD.Entities;
using SharedEntities;
using System.Text.Json;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Net.Http.Headers;
using SharedEntities.BD.Data;
using System.Text.Json.Serialization;
using SharedEntities.Data.Users;

namespace RCLGeral.API.Services;

public class ApiService : IApiServices
{
    private readonly ILogger<ApiService> _logger;
    private readonly HttpClient _httpClient = new();
    JsonSerializerOptions _serializerOptions;

    private List<Produto> produtos;
    private List<Categoria> categorias;
    private Produto produto;

    public ApiService(ILogger<ApiService> logger)
    {
        _logger = logger;
        _serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        categorias = new List<Categoria>();
        produtos = new List<Produto>();
    }


    // ------------------------------------ Categorias ------------------------------------ //
    public async Task<List<Categoria>?> GetCategorias()
    {
        string endpoint = $"api/Categorias";

        try
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"{AppConfig.BaseUrl}{endpoint}");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string content = "";
                content = await httpResponseMessage.Content.ReadAsStringAsync();
                categorias = JsonSerializer.Deserialize<List<Categoria>>(content, _serializerOptions)!;
            }

            return categorias;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }


    // ------------------------------------- Produtos ------------------------------------- //

    public async Task<List<Produto>?> GetProdutos(string tipoProduto, int? categoriaID = null)
    {
        string endpoint = $"api/Produtos?tipoProduto={tipoProduto}";

        if (categoriaID.HasValue)
        {
            endpoint += $"&categoriaID={categoriaID.Value}";
        }

        try
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"{AppConfig.BaseUrl}{endpoint}");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string content = await httpResponseMessage.Content.ReadAsStringAsync();
                List<Produto> produtos = JsonSerializer.Deserialize<List<Produto>>(content, _serializerOptions)!;
                return produtos;
            }
            else if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                string errorContent = await httpResponseMessage.Content.ReadAsStringAsync();
                Console.WriteLine($"Erro de requisição: {errorContent}");
                return null;
            }
            else
            {
                Console.WriteLine($"Erro na requisição: {httpResponseMessage.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exceção ao obter produtos: {ex.Message}");
            return null;
        }
    }

    public async Task<Produto> GetProdutoById(int id)
    {
        string endpoint = $"api/Produtos/{id}";

        try
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"{AppConfig.BaseUrl}{endpoint}");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string content = "";
                content = await httpResponseMessage.Content.ReadAsStringAsync();
                produto = JsonSerializer.Deserialize<Produto>(content, _serializerOptions)!;
            }

            return produto;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }



    // ----------------------------------- Utilizadores ----------------------------------- //

    public async Task<ApiResponse<LoginResult>> Login(string email, string password)
    {
        try
        {
            string endpoint = $"api/Utilizadores/Login";
            var loginModel = new Login { Email = email, Password = password };

            var response = await _httpClient.PostAsJsonAsync($"{AppConfig.BaseUrl}{endpoint}", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var loginResult = JsonSerializer.Deserialize<LoginResult>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (loginResult != null)
                {
                    //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.accessToken);
                    //Console.WriteLine($"Token: {loginResult.accessToken}"); // Log do token
                }

                return new ApiResponse<LoginResult>
                {
                    Data = loginResult!,
                    ErrorMessage = null
                };
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new ApiResponse<LoginResult>
                {
                    Data = null,
                    ErrorMessage = "Utilizador não encontrado"
                };
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return new ApiResponse<LoginResult>
                {
                    Data = null,
                    ErrorMessage = "Erro: Login Inválido!"
                };
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                return new ApiResponse<LoginResult>
                {
                    Data = null,
                    ErrorMessage = $"Role inválido: Apenas clientes podem fazer login!"
                };
            }
            else
            {
                return new ApiResponse<LoginResult>
                {
                    Data = null,
                    ErrorMessage = $"Erro no login: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao logar o utilizador: {ex.Message}");
            return new ApiResponse<LoginResult>
            {
                Data = null,
                ErrorMessage = $"Erro inesperado: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<ApplicationUser>> GetProfile()
    {
        string endpoint = "api/Utilizadores/Profile";

        try
        {
            using var response = await _httpClient.GetAsync($"{AppConfig.BaseUrl}{endpoint}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                };
                var user = JsonSerializer.Deserialize<ApplicationUser>(content, options);

                return new ApiResponse<ApplicationUser>
                {
                    Data = user!,
                    ErrorMessage = null
                };
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new ApiResponse<ApplicationUser>
                {
                    Data = null,
                    ErrorMessage = "Perfil não encontrado"
                };
            }
            else
            {
                return new ApiResponse<ApplicationUser>
                {
                    Data = null,
                    ErrorMessage = $"Erro ao obter perfil: {response.StatusCode}"
                };
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de rede ao obter perfil do utilizador");
            return new ApiResponse<ApplicationUser>
            {
                Data = null,
                ErrorMessage = "Erro de conexão ao servidor"
            };
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erro ao deserializar resposta do perfil do utilizador");
            return new ApiResponse<ApplicationUser>
            {
                Data = null,
                ErrorMessage = "Erro ao processar dados do servidor"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao obter perfil do utilizador");
            return new ApiResponse<ApplicationUser>
            {
                Data = null,
                ErrorMessage = "Ocorreu um erro inesperado"
            };
        }
    }

    public async Task<ApiResponse<bool>> Registar(Register registo)
    {
        string endpoint = $"{AppConfig.BaseUrl}api/Utilizadores/Register";

        try
        {
            var json = JsonSerializer.Serialize(registo, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool>
                {
                    Data = true,
                    ErrorMessage = null
                };
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new ApiResponse<bool>
                {
                    Data = false,
                    ErrorMessage = $"Erro no registo (BadRequest): {errorContent}"
                };
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                return new ApiResponse<bool>
                {
                    Data = false,
                    ErrorMessage = "Utilizador já existe"
                };
            }
            else
            {
                return new ApiResponse<bool>
                {
                    Data = false,
                    ErrorMessage = $"Erro no registo: {response.StatusCode}"
                };
            }
        }

        catch (HttpRequestException ex)
        {
            _logger.LogError($"Erro de rede ao registar o utilizador: {ex.Message}");
            return new ApiResponse<bool>
            {
                Data = false,
                ErrorMessage = $"Erro de rede: {ex.Message}"
            };
        }
        catch (JsonException ex)
        {
            _logger.LogError($"Erro ao serializar dados de registo: {ex.Message}");
            return new ApiResponse<bool>
            {
                Data = false,
                ErrorMessage = $"Erro ao processar dados: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro inesperado ao registar o utilizador: {ex.Message}");
            return new ApiResponse<bool>
            {
                Data = false,
                ErrorMessage = $"Erro inesperado: {ex.Message}"
            };
        }
    }



    // ------------------------------------ Encomendas ------------------------------------ //

    public async Task<ApiResponse<List<Encomenda>>> GetEncomendas(string? clienteEmail = null)
    {
        string endpoint = $"api/Encomendas";
        if (clienteEmail != null)
        {
            endpoint += $"?clientEmail={clienteEmail}";
        }

        var (encomendas, errorMessage) = await GetAsync<List<Encomenda>>(endpoint);

        return new ApiResponse<List<Encomenda>>
        {
            Data = encomendas,
            ErrorMessage = errorMessage
        };
    }

    
    public async Task<ApiResponse<Encomenda>> GetDetalheEncomenda(int id)
    {
        string endpoint = $"api/Encomendas/{id}";

        var (encomenda, errorMessage) = await GetAsync<Encomenda>(endpoint);

        if (encomenda is null)
        {
            Console.WriteLine($"Erro ao obter encomenda: {errorMessage}");
        }
        
        return new ApiResponse<Encomenda>
        {
            Data = encomenda,
            ErrorMessage = errorMessage
        };
    }


    public async Task<ApiResponse<Encomenda>> AdicionarEncomenda(Encomenda novaEncomenda)
    {
        string endpoint = $"api/Encomendas";

        try
        {
            var json = JsonSerializer.Serialize(novaEncomenda, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{AppConfig.BaseUrl}{endpoint}", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var encomendaAdicionada = JsonSerializer.Deserialize<Encomenda>(responseContent, _serializerOptions);

                return new ApiResponse<Encomenda>
                {
                    Data = encomendaAdicionada,
                    ErrorMessage = null
                };
            }
            else
            {
                return new ApiResponse<Encomenda>
                {
                    Data = null,
                    ErrorMessage = $"Erro ao adicionar encomenda: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            return new ApiResponse<Encomenda>
            {
                Data = null,
                ErrorMessage = $"Erro inesperado: {ex.Message}"
            };
        }
    }



    // ------------------------------------ Pagamentos ------------------------------------ //

    
    public async Task<ApiResponse<List<Pagamento>>> GetPagamentos(string clienteEmail)
    {
        string endpoint = $"api/Pagamentos";
        if (clienteEmail != null){
            endpoint += $"?clienteEmail={clienteEmail}";
        }

        var (pagamentos, errorMessage) = await GetAsync<List<Pagamento>>(endpoint);

        return new ApiResponse<List<Pagamento>>
        {
            Data = pagamentos,
            ErrorMessage = errorMessage
        };
    }


    public async Task<ApiResponse<Pagamento>> GetPagamentoPorEncomenda(int id)
    {
        string endpoint = $"api/Pagamentos/{id}";

        var (pagamento, errorMessage) = await GetAsync<Pagamento>(endpoint);

        if (pagamento is null)
        {
            Console.WriteLine($"Erro ao obter Pagamento: {errorMessage}");
        }

        return new ApiResponse<Pagamento>
        {
            Data = pagamento,
            ErrorMessage = errorMessage
        };
    }


    public async Task<ApiResponse<Pagamento>> AdicionarPagamento(Pagamento novoPagamento)
    {
        string endpoint = $"api/Pagamentos";

        try
        {
            var json = JsonSerializer.Serialize(novoPagamento, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{AppConfig.BaseUrl}{endpoint}", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var pagamentoAdicionado = JsonSerializer.Deserialize<Pagamento>(responseContent, _serializerOptions);

                return new ApiResponse<Pagamento>
                {
                    Data = pagamentoAdicionado,
                    ErrorMessage = null
                };
            }
            else
            {
                return new ApiResponse<Pagamento>
                {
                    Data = null,
                    ErrorMessage = $"Erro ao adicionar Pagamento: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            return new ApiResponse<Pagamento>
            {
                Data = null,
                ErrorMessage = $"Erro inesperado: {ex.Message}"
            };
        }
    }





    public async Task<HttpResponseMessage> PostRequest(string enderecoURL, HttpContent content)
    {
        try
        {
            var result = await _httpClient.PostAsync(enderecoURL, content);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao enviar requisição POST para enderecoURL: {ex.Message}");
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }

    public async Task<(T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(AppConfig.BaseUrl + endpoint);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);
                return (data ?? Activator.CreateInstance<T>(), null);
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    string errorMessage = "UnAuthorized";
                    _logger.LogWarning(errorMessage);
                    return (default, errorMessage);
                }

                string generalErrorMessage = $"Erro na requisição: {response.ReasonPhrase}";
                _logger.LogWarning(generalErrorMessage);
                return (default, generalErrorMessage);
            }
        }
        catch (HttpRequestException ex)
        {
            string errMessage = $"Erro de requisição HTTP: {ex.Message}";
            _logger.LogWarning(errMessage);
            return (default, errMessage);
        }
        catch (JsonException ex)
        {
            string errMessage = $"Erro de desserialização Json: {ex.Message}";
            _logger.LogWarning(ex.Message);
            return (default, errMessage);
        }
        catch (Exception ex)
        {
            string errMessage = $"Erro inesperado: {ex.Message}";
            _logger.LogWarning(ex.Message);
            return (default, errMessage);
        }
    }
}
