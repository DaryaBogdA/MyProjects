using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace paseka.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public bool IsRoom { get; set; }
        public bool IsAvailable { get; set; }
    }
}
