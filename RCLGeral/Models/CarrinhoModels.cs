namespace RCLGeral.Models
{
    /// <summary>
    /// Item do carrinho de compras (guardado localmente)
    /// </summary>
    public class CarrinhoItem
    {
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; } = string.Empty;
        public string? ProdutoImagem { get; set; }
        public decimal PrecoUnitario { get; set; }
        public int Quantidade { get; set; }
        public int StockDisponivel { get; set; }

        public decimal Subtotal => PrecoUnitario * Quantidade;

        public bool PodeAumentar => Quantidade < StockDisponivel;
        public bool PodeDiminuir => Quantidade > 1;
    }

    /// <summary>
    /// Estado do carrinho de compras
    /// </summary>
    public class Carrinho
    {
        public List<CarrinhoItem> Itens { get; set; } = new();

        public int TotalItens => Itens.Sum(i => i.Quantidade);
        public decimal Total => Itens.Sum(i => i.Subtotal);

        public void AdicionarItem(ProdutoModel produto, int quantidade = 1)
        {
            var itemExistente = Itens.FirstOrDefault(i => i.ProdutoId == produto.Id);
            
            if (itemExistente != null)
            {
                itemExistente.Quantidade += quantidade;
            }
            else
            {
                Itens.Add(new CarrinhoItem
                {
                    ProdutoId = produto.Id,
                    ProdutoNome = produto.Nome,
                    ProdutoImagem = produto.ImagemUrl,
                    PrecoUnitario = produto.Preco,
                    Quantidade = quantidade,
                    StockDisponivel = (int)produto.Stock  // Cast decimal to int
                });
            }
        }

        public void RemoverItem(int produtoId)
        {
            var item = Itens.FirstOrDefault(i => i.ProdutoId == produtoId);
            if (item != null)
            {
                Itens.Remove(item);
            }
        }

        public void AtualizarQuantidade(int produtoId, int quantidade)
        {
            var item = Itens.FirstOrDefault(i => i.ProdutoId == produtoId);
            if (item != null)
            {
                if (quantidade <= 0)
                {
                    Itens.Remove(item);
                }
                else
                {
                    item.Quantidade = Math.Min(quantidade, item.StockDisponivel);
                }
            }
        }

        public void Limpar()
        {
            Itens.Clear();
        }
    }

    public class ItemEncomenda
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }
}
