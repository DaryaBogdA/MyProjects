using System;

namespace Avto.Models
{
    public enum OrderStatus
    {
        active,
        completed,
        cancelled
    }

    public class Order
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalCost { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleNumber { get; set; }
    }
}