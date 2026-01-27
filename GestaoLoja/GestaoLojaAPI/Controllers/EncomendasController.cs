using RCLGeral.Entities;
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

        public EncomendasController(IEncomendasRepository encomendasRepository)
        {
            _encomendasRepository = encomendasRepository;
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

        // GET api/Encomendas/cliente/{clienteId}
        [HttpGet("cliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<Encomendas>>> GetByUserId(string clienteId)
        {
            var encomendas = await _encomendasRepository.GetEncomendasByUserIdAsync(clienteId);
            return Ok(encomendas);
        }

        // POST api/Encomendas
        [HttpPost]
        public async Task<ActionResult<Encomendas>> CreateEncomenda([FromBody] Encomendas encomenda)
        {
            if (encomenda == null || encomenda.Pedidos == null || !encomenda.Pedidos.Any())
            {
                return BadRequest("Encomenda ou pedidos inválidos.");
            }

            // Definir a data de criação
            encomenda.DataCriacao = DateTime.Now;

            // Preparar os pedidos antes de salvar (não definir Id - deixar o EF gerar)
            foreach (var pedido in encomenda.Pedidos)
            {
                pedido.Id = 0; // Garantir que o Id é 0 para que o EF gere automaticamente
                pedido.IdUser = encomenda.ClienteId;
                pedido.ValorTotal = pedido.PrecoUnitario * pedido.Quantidade;
            }

            // Salvar a encomenda - EF Core vai salvar os Pedidos automaticamente
            var createdEncomenda = await _encomendasRepository.CreateEncomendaAsync(encomenda);

            return CreatedAtAction(nameof(Get), new { id = createdEncomenda.Id }, createdEncomenda);
        }

        // POST api/Encomendas/{id}/pagar
        [HttpPost("{id}/pagar")]
        public async Task<IActionResult> PagarEncomenda(int id, [FromBody] PagamentoRequest request)
        {
            var success = await _encomendasRepository.PagarEncomendaAsync(id, request?.MetodoPagamento ?? "");
            if (!success)
            {
                return BadRequest("Não foi possível processar o pagamento.");
            }
            return Ok(new { message = "Pagamento registado com sucesso!" });
        }

        // POST api/Encomendas/{id}/cancelar
        [HttpPost("{id}/cancelar")]
        public async Task<IActionResult> CancelarEncomenda(int id)
        {
            var success = await _encomendasRepository.CancelarEncomendaAsync(id);
            if (!success)
            {
                return BadRequest("Não foi possível cancelar a encomenda.");
            }
            return Ok(new { message = "Encomenda cancelada com sucesso!" });
        }
    }

    public class PagamentoRequest
    {
        public string? MetodoPagamento { get; set; }
    }
}
