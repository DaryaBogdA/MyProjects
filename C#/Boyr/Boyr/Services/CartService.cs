using Boyr.Delegates; 
using Boyr.Entities;
using Boyr.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Boyr.Services
{
    public class CartService : ICartService
    {
        private static CartService _instance;
        private List<CartItem> _items = new List<CartItem>();

        private CartService() { }

        public static CartService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CartService();
                return _instance;
            }
        }

        public event CartChangedEventHandler CartChanged;

        public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
        public decimal TotalAmount => _items.Sum(i => i.TotalPrice);
        public int TotalItems => _items.Sum(i => i.Quantity);

        public bool AddToCart(Product product, int quantity = 1)
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

            OnCartChanged();
            return true;
        }

        public void RemoveFromCart(int productId)
        {
            _items.RemoveAll(i => i.Product.Id == productId);
            OnCartChanged();
        }

        public bool UpdateQuantity(int productId, int newQuantity)
        {
            var item = _items.FirstOrDefault(i => i.Product.Id == productId);
            if (item == null) return false;
            if (newQuantity <= 0)
            {
                _items.Remove(item);
                OnCartChanged();
                return true;
            }
            if (newQuantity > item.Product.Quantity)
            {
                return false;
            }
            item.Quantity = newQuantity;
            OnCartChanged();
            return true;
        }

        public void Clear()
        {
            _items.Clear();
            OnCartChanged();
        }

        public bool ContainsProduct(int productId)
        {
            return _items.Any(i => i.Product.Id == productId);
        }

        private void OnCartChanged()
        {
            CartChanged?.Invoke(TotalItems, TotalAmount);
        }
    }
}