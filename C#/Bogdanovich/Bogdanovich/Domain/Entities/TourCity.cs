namespace Bogdanovich.Domain.Entities
{
    public class TourCity : Entity
    {
        public int TourId { get; set; }
        public int CityId { get; set; }
        public int DaysCount { get; set; } = 1;
        public int SortOrder { get; set; } = 0;

        public virtual Tour? Tour { get; set; }
        public virtual City? City { get; set; }
    }
}
