using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boyr.Entities
{
    public class Sale
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime SaleDate { get; set; }
        public string CustomerName { get; set; }
        public User User { get; set; }
        public ICollection<SaleItem> SaleItems { get; set; }
    }
}
