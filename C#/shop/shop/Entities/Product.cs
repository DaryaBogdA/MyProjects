using System.Collections.Generic;

namespace shop.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        public Category Category { get; set; }
        public ICollection<SaleItem> SaleItems { get; set; }
    }
}