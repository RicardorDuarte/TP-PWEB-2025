using RCLGeral.Models;

namespace RCLGeral.Services
{
    /// <summary>
    /// Serviço para gerir o carrinho de compras (estado local)
    /// </summary>
    public interface ICarrinhoService
    {
        Carrinho Carrinho { get; }
        event Action? OnCarrinhoChanged;
        void AdicionarProduto(ProdutoModel produto, int quantidade = 1);
        void RemoverProduto(int produtoId);
        void AtualizarQuantidade(int produtoId, int quantidade);
        void Limpar();
        int GetQuantidadeTotal();
        decimal GetTotal();
    }

    public class CarrinhoService : ICarrinhoService
    {
        public Carrinho Carrinho { get; } = new();

        public event Action? OnCarrinhoChanged;

        public void AdicionarProduto(ProdutoModel produto, int quantidade = 1)
        {
            Carrinho.AdicionarItem(produto, quantidade);
            OnCarrinhoChanged?.Invoke();
        }

        public void RemoverProduto(int produtoId)
        {
            Carrinho.RemoverItem(produtoId);
            OnCarrinhoChanged?.Invoke();
        }

        public void AtualizarQuantidade(int produtoId, int quantidade)
        {
            Carrinho.AtualizarQuantidade(produtoId, quantidade);
            OnCarrinhoChanged?.Invoke();
        }

        public void Limpar()
        {
            Carrinho.Limpar();
            OnCarrinhoChanged?.Invoke();
        }

        public int GetQuantidadeTotal() => Carrinho.TotalItens;

        public decimal GetTotal() => Carrinho.Total;
    }
}
