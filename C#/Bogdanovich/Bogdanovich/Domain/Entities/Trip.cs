namespace Bogdanovich.Domain.Entities
{
    public class Trip : Entity
    {
        public int UserId { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public DateOnly? DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }
        public int PeopleCount { get; set; } = 1;

        public virtual User? User { get; set; }
        public virtual Country? Country { get; set; }
        public virtual City? City { get; set; }
        public virtual ICollection<TripItem> Items { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
