using Boyr.Entities;
using System.Collections.Generic;

namespace Boyr.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }      
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }                  
        public string Metal { get; set; }                    
        public int Purity { get; set; }                    
        public decimal Weight { get; set; }                   
        public string Gemstone { get; set; }                   
        public string GemCharacteristics { get; set; }        
        public int Quantity { get; set; }                       
        public string ImageUrl { get; set; }             
        public Category Category { get; set; }
        public ICollection<SaleItem> SaleItems { get; set; }
    }
}