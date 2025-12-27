using SharedEntities.BD.Data;
using SharedEntities.BD.Entities;
using SharedEntities.Data.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCLGeral.API.Services;
public interface IApiServices
{
    Task<List<Categoria>?> GetCategorias();

    Task<List<Produto>?> GetProdutos(string tipoProduto, int? categoriaID = null);

    Task<Produto> GetProdutoById(int id);



    Task<ApiResponse<LoginResult>> Login(string email, string password);

    Task<ApiResponse<bool>> Registar(Register registo);

    Task<ApiResponse<ApplicationUser>> GetProfile();



    Task<ApiResponse<List<Encomenda>>> GetEncomendas(string? clienteID = null);

    Task<ApiResponse<Encomenda>> GetDetalheEncomenda(int id);

    Task<ApiResponse<Encomenda>> AdicionarEncomenda(Encomenda novaEncomenda);



    Task<ApiResponse<List<Pagamento>>> GetPagamentos(string clienteEmail);

    Task<ApiResponse<Pagamento>> GetPagamentoPorEncomenda(int id);

    Task<ApiResponse<Pagamento>> AdicionarPagamento(Pagamento novoPagamento);



    public Task<(T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint);

    Task<HttpResponseMessage> PostRequest(string enderecoURL, HttpContent content);


    //public Task<(bool Data, string? ErrorMessage)> ActualizaFavorito(string acao, int produtoId);

    //public Task<List<Produto>> GetFavoritos(string utilizadorId);
}

