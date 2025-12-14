using GestaoLojaAPI.Context;
using GestaoLojaAPI.Data;
using GestaoLojaAPI.Entities;
using Microsoft.EntityFrameworkCore;
using RCLAPI.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoLojaAPI.Repositories
{
    public class CarrinhoRepository : ICarrinhoRepository
    {
        private readonly AppDbContext _context;

        public CarrinhoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AdicionarOuAtualizarItem(ItemCarrinhoCompra item)
        {
            var itemExistente = await _context.ItemCarrinhoCompra
                .FirstOrDefaultAsync(i => i.ProdutoId == item.ProdutoId && i.UserId == item.UserId);

            if (itemExistente != null)
            {
                // Atualiza a quantidade e o valor total
                itemExistente.Quantidade += item.Quantidade;
                itemExistente.ValorTotal = itemExistente.Quantidade * itemExistente.PrecoUnitario;

                _context.ItemCarrinhoCompra.Update(itemExistente);
            }
            else
            {
                // Adiciona um novo item
                await _context.ItemCarrinhoCompra.AddAsync(item);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<ItemCarrinhoCompra>> ObterCarrinhoPorUser(string userId)
        {
            // Filtra os itens do carrinho pela UserId e retorna a lista de itens
            return await _context.ItemCarrinhoCompra
                                 .Where(x => x.UserId == userId)
                                 .ToListAsync();
        }

        public async Task<bool> RemoverItem(int id)
        {
            var item = await _context.ItemCarrinhoCompra.FindAsync(id);
            if (item == null)
            {
                return false;
            }

            _context.ItemCarrinhoCompra.Remove(item);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> AtualizarItem(ItemCarrinhoCompra item)
        {
            var itemExistente = await _context.ItemCarrinhoCompra
                .FirstOrDefaultAsync(i => i.ProdutoId == item.ProdutoId && i.UserId == item.UserId);

            if (itemExistente != null)
            {
                // Atualiza a quantidade e o valor total
                itemExistente.Quantidade = item.Quantidade;
                itemExistente.ValorTotal = itemExistente.Quantidade * itemExistente.PrecoUnitario;

                _context.ItemCarrinhoCompra.Update(itemExistente);
            }
            else
            {
                // Adiciona um novo item
                await _context.ItemCarrinhoCompra.AddAsync(item);
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
