using System;

namespace Avto.Models
{
    public enum FuelType
    {
        Бензин,
        Дизель,
        Электричество,
        Гибрид,
        Газ
    }

    public enum VehicleStatus
    {
        В_наличии,
        Продан,
        В_ремонте
    }

    public class Vehicle
    {
        public int Id { get; set; }
        public int? VehicleTypeId { get; set; }
        public string Make { get; set; }  
        public int? Year { get; set; } 
        public string Number { get; set; }
        public string Color { get; set; }
        public FuelType FuelType { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public VehicleStatus Status { get; set; }
        public string Notes { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsAvailable { get; set; }
        public decimal PricePerHour { get; set; } 
        public string VehicleTypeName { get; set; }

    }
}