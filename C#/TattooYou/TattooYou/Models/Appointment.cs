using System;

namespace TattooYou.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public int? StyleId { get; set; }  
        public int? MasterId { get; set; }
        public string Size { get; set; } 
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public User User { get; set; }
        public Service Service { get; set; }
        public Style Style { get; set; }
        public Master Master { get; set; }
    }
}