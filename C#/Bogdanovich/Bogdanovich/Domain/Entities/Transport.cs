using Bogdanovich.Domain.Enums;

namespace Bogdanovich.Domain.Entities
{
    public class Transport : Entity
    {
        public TransportTypeEnum Type { get; set; }
        public int FromCityId { get; set; }
        public int ToCityId { get; set; }
        public DateTime? DepartureTime { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public decimal? Price { get; set; }
        public string? Company { get; set; }
        public string? Info { get; set; }

        public virtual City? FromCity { get; set; }
        public virtual City? ToCity { get; set; }
    }
}
