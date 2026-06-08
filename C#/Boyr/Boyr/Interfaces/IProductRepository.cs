using Boyr.Entities;
using System.Collections.Generic;

namespace Boyr.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        List<Product> GetByCategory(int categoryId);
        List<Product> GetByMetal(string metal);
        List<Product> GetByPriceRange(decimal min, decimal max);
        void UpdateQuantity(int productId, int newQuantity);
        bool IsInStock(int productId, int requestedQuantity);
    }
}