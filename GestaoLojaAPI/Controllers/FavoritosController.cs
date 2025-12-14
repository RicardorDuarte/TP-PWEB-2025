using GestaoLojaAPI.Entities;
using GestaoLojaAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoLojaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritosController : ControllerBase
    {
        private readonly IFavoritosRepository _favoritosRepository;
        private readonly IProdutoRepository _produtosRepository;  // Adicionei o repositório de produtos

        public FavoritosController(IFavoritosRepository favoritosRepository, IProdutoRepository produtosRepository)
        {
            _favoritosRepository = favoritosRepository;
            _produtosRepository = produtosRepository;  // Inicializando o repositório de produtos
        }

        // GET: /api/Favoritos/{clienteId}
        [HttpGet("{clienteId}")]
        public async Task<IActionResult> GetFavoritos(string clienteId)
        {
            var favoritos = await _favoritosRepository.GetFavoritosAsync(clienteId);

            if (favoritos == null || !favoritos.Any())
            {
                return NotFound(new { Mensagem = "Nenhum favorito encontrado para este cliente." });
            }

            return Ok(favoritos);
        }

        // PUT: /api/Favoritos/{produtoId}/{acao}/{UserId}
        [HttpPut("{produtoId}/{acao}/{UserId}")]
        public async Task<IActionResult> AtualizaFavorito(int produtoId, string acao, string UserId)
        {
            // Verifique se o produto existe
            var produto = await _produtosRepository.GetProdutoByIdAsync(produtoId);  // Supondo que exista um método GetProdutoByIdAsync
            if (produto == null)
            {
                return NotFound(new { Mensagem = "Produto não encontrado." });
            }

            var favorito = await _favoritosRepository.GetFavoritoAsync(produtoId, UserId);

            if (acao == "heartfill") // Adicionar aos favoritos
            {
                if (favorito == null)
                {
                    favorito = new ProdutoFavorito
                    {
                        ProdutoId = produtoId,
                        ClienteId = UserId,
                        Efavorito = true
                    };
                    await _favoritosRepository.AdicionarFavoritoAsync(favorito);
                }
                else
                {
                    favorito.Efavorito = true; // Marca como favorito
                    await _favoritosRepository.AtualizarFavoritoAsync(favorito);
                }
            }
            else if (acao == "heartsimples") // Remover dos favoritos
            {
                if (favorito != null)
                {
                    favorito.Efavorito = false; // Desmarca como favorito
                    await _favoritosRepository.AtualizarFavoritoAsync(favorito);
                }
                else
                {
                    return NotFound(new { Mensagem = "Produto favorito não encontrado." });
                }
            }
            else
            {
                return BadRequest(new { Mensagem = "Ação inválida. Use 'heartfill' ou 'heartsimples'." });
            }

            return Ok(new { Sucesso = true, Mensagem = "Ação realizada com sucesso." });
        }
    }
}
