namespace Bogdanovich.Domain.Entities
{
    public class Attraction : Entity
    {
        public int CityId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? PhotoUrl { get; set; }
        public int Popularity { get; set; } = 0;

        public virtual City? City { get; set; }
    }
}
