namespace Bogdanovich.Domain.Entities
{
    public class Excursion : Entity
    {
        public int CityId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? DurationHours { get; set; }
        public string? MeetingPoint { get; set; }
        public decimal? Price { get; set; }
        public string? PhotoUrl { get; set; }

        public virtual City? City { get; set; }
        public virtual ICollection<ExcursionDate> Dates { get; set; }
    }
}
