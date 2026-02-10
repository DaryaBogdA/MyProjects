using Bogdanovich.Domain.Enums;

namespace Bogdanovich.Domain.Entities
{
    public class Booking : Entity
    {
        public int UserId { get; set; }
        public int TripId { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatusEnum Status { get; set; } = BookingStatusEnum.PENDING;
        public string? PaymentMethod { get; set; }

        public virtual User? User { get; set; }
        public virtual Trip? Trip { get; set; }
    }
}
