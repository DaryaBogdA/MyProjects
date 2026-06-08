using Boyr.Entities;
using Boyr.Delegates; 
using System.Collections.Generic;

namespace Boyr.Interfaces
{
    public interface ICartService
    {
        IReadOnlyList<CartItem> Items { get; }
        decimal TotalAmount { get; }
        int TotalItems { get; }

        bool AddToCart(Product product, int quantity = 1);
        void RemoveFromCart(int productId);
        bool UpdateQuantity(int productId, int newQuantity);
        void Clear();
        bool ContainsProduct(int productId);

        event CartChangedEventHandler CartChanged;
    }
}