using System;

namespace SanatoriumStaro.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}