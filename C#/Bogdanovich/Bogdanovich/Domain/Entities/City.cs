namespace Bogdanovich.Domain.Entities
{
    public class City : Entity
    {
        public int CountryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Popularity { get; set; } = 0;

        public virtual Country? Country { get; set; }
        public virtual ICollection<Attraction> Attractions { get; set; }
        public virtual ICollection<Hotel> Hotels { get; set; }
        public virtual ICollection<Excursion> Excursions { get; set; }
    }
}
