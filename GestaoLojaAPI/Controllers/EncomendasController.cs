using GestaoLojaAPI.Entities;
using GestaoLojaAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GestaoLojaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncomendasController : ControllerBase
    {
        private readonly IEncomendasRepository _encomendasRepository;
        private readonly IPedidosRepository _pedidosRepository;

        public EncomendasController(IEncomendasRepository encomendasRepository, IPedidosRepository pedidosRepository)
        {
            _encomendasRepository = encomendasRepository;
            _pedidosRepository = pedidosRepository;
        }

        // GET api/Encomendas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Encomendas>>> Get()
        {
            var encomendas = await _encomendasRepository.GetAllEncomendasAsync();
            if (encomendas == null)
            {
                return NotFound();
            }
            return Ok(encomendas);
        }

        // GET api/Encomendas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Encomendas>> Get(int id)
        {
            var encomenda = await _encomendasRepository.GetEncomendaByIdAsync(id);
            if (encomenda == null)
            {
                return NotFound();
            }
            return Ok(encomenda);
        }

        // POST api/Encomendas
        [HttpPost]
        public async Task<ActionResult<Encomendas>> CreateEncomenda([FromBody] Encomendas encomenda)
        {
            if (encomenda == null || encomenda.Pedidos == null || !encomenda.Pedidos.Any())
            {
                return BadRequest("Encomenda ou pedidos inválidos.");
            }

            // Guardar a encomenda primeiro
            encomenda.DataCriacao = DateTime.Now; // Definir a data de criação da encomenda
            var createdEncomenda = await _encomendasRepository.CreateEncomendaAsync(encomenda);

            // Agora que a encomenda tem um ID, associar o EncomendaId aos pedidos
            foreach (var pedido in createdEncomenda.Pedidos)
            {
                pedido.EncomendaId = createdEncomenda.Id; // Associar o EncomendaId ao pedido
                pedido.IdUser = createdEncomenda.ClienteId; // Associar o IdUser ao pedido
            }

            // Salvar os pedidos associados à encomenda
            await _pedidosRepository.AddPedidosAsync(createdEncomenda.Pedidos);

            return CreatedAtAction(nameof(Get), new { id = createdEncomenda.Id }, createdEncomenda);
        }
    }
}
