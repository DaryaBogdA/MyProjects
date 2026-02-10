namespace Bogdanovich.Domain.Entities
{
    public class Country : Entity
    {
        public string? Name { get; set; }
        public int Popularity { get; set; } = 0;

        public virtual ICollection<City> Cities { get; set; }
        public virtual ICollection<TourCountry> TourCountries { get; set; }
    }
}
