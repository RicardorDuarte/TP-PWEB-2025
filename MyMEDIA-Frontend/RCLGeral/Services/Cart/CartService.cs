using SharedEntities.BD.Entities;
using SharedEntities.Data.Shop;

namespace RCLGeral.Services.Cart;

public class CartService
{
    public List<CartItem> Items { get; private set; } = new List<CartItem>();

    public event Action? OnChange;

    public Checkout? CheckoutInfo { get; private set; }
    public Encomenda? Encomenda { get; private set; }

    public void AddItem(CartItem item){
        var existingItem = Items.FirstOrDefault(i => i.Produto.ID == item.Produto.ID);
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            Items.Add(item);
        }
        OnChange?.Invoke();
    }

    public void RemoveItem(CartItem item){
        Items.Remove(item);
        OnChange?.Invoke();
    }

    public void UpdateQuantity(CartItem item, int quantity){
        var existingItem = Items.FirstOrDefault(i => i.Produto.ID == item.Produto.ID);
        if (existingItem != null)
        {
            existingItem.Quantity = quantity;
            OnChange?.Invoke();
        }
    }

    public List<CartItem> GetCartItems()
    {
        return Items;
    }

    public void ClearCart(){
        Items.Clear();
        OnChange?.Invoke();
    }

    public void SetCheckoutInfo(Checkout checkout)
    {
        CheckoutInfo = checkout;
        OnChange?.Invoke();
    }

    public void SetEncomenda(Encomenda encomenda){
        Encomenda = encomenda;
        OnChange?.Invoke();
    }
}
