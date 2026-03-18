using System;

namespace Avto.Models
{
    public class VehicleType
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}