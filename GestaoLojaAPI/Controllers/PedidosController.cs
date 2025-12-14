using GestaoLojaAPI.Entities;
using GestaoLojaAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLojaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidosRepository _pedidosRepository;

        public PedidosController(IPedidosRepository pedidosRepository)
        {
            _pedidosRepository = pedidosRepository;
        }

        // GET api/Pedidos/DetalhesPedido/{pedidoId}
        [HttpGet("DetalhesPedido/{pedidoId}")]
        public async Task<ActionResult<Pedidos>> GetDetalhesPedido(int pedidoId)
        {
            var pedido = await _pedidosRepository.GetDetalhesPedidoAsync(pedidoId);
            if (pedido == null)
            {
                return NotFound();
            }
            return Ok(pedido);
        }

        // GET api/Pedidos/PedidosPorUtilizador/{userId}
        [HttpGet("PedidosPorUtilizador/{userId}")]
        public async Task<ActionResult<IEnumerable<Pedidos>>> GetPedidosPorUtilizador(string userId)
        {
            var pedidos = await _pedidosRepository.GetPedidosPorUtilizadorAsync(userId);
            if (pedidos == null)
            {
                return NotFound();
            }
            return Ok(pedidos);
        }

        // POST api/Pedidos
        [HttpPost]
        public async Task<ActionResult<Pedidos>> CreatePedido([FromBody] Pedidos pedido)
        {
            if (pedido == null)
            {
                return BadRequest("Pedido inválido.");
            }

            var createdPedido = await _pedidosRepository.CreatePedidoAsync(pedido);
            return CreatedAtAction(nameof(GetDetalhesPedido), new { pedidoId = createdPedido.Id }, createdPedido);
        }
    }
}
