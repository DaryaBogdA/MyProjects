using Bogdanovich.Domain.Enums;

namespace Bogdanovich.Domain.Entities
{
    public class Tour : Entity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public TourTypeEnum Type { get; set; }
        public int DurationDays { get; set; } = 3;
        public decimal PriceTotal { get; set; }
        public string? PhotoUrl { get; set; }
        public int Popularity { get; set; } = 0;

        public virtual ICollection<TourCountry> TourCountries { get; set; }
        public virtual ICollection<TourCity> TourCities { get; set; }
    }
}
