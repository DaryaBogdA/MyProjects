namespace Bogdanovich.Domain.Entities
{
    public class TourCountry : Entity
    {
        public int TourId { get; set; }
        public int CountryId { get; set; }
        public int DaysCount { get; set; } = 1;
        public int SortOrder { get; set; } = 0;

        public virtual Tour? Tour { get; set; }
        public virtual Country? Country { get; set; }
    }
}
