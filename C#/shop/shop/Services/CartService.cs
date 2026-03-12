using System.Collections.Generic;
using System.Linq;
using shop.Entities;

namespace shop.Services
{
    public static class CartService
    {
        private static List<CartItem> _items = new List<CartItem>();

        public static IReadOnlyList<CartItem> Items => _items.AsReadOnly();

        public static bool AddToCart(Product product, int quantity = 1)
        {
            if (product == null) return false;
            var existing = _items.FirstOrDefault(i => i.Product.Id == product.Id);
            int currentQtyInCart = existing?.Quantity ?? 0;
            int newTotal = currentQtyInCart + quantity;
            if (newTotal > product.Quantity)
            {
                return false;
            }
            if (existing != null)
                existing.Quantity = newTotal;
            else
                _items.Add(new CartItem { Product = product, Quantity = quantity });
            return true;
        }

        public static void RemoveFromCart(int productId)
        {
            _items.RemoveAll(i => i.Product.Id == productId);
        }

        public static bool UpdateQuantity(int productId, int newQuantity)
        {
            var item = _items.FirstOrDefault(i => i.Product.Id == productId);
            if (item == null) return false;
            if (newQuantity <= 0)
            {
                _items.Remove(item);
                return true;
            }
            if (newQuantity > item.Product.Quantity)
            {
                return false;
            }
            item.Quantity = newQuantity;
            return true;
        }

        public static void Clear()
        {
            _items.Clear();
        }

        public static decimal TotalAmount => _items.Sum(i => i.TotalPrice);
    }
}