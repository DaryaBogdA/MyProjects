using Bogdanovich.Domain.Enums;

namespace Bogdanovich.Domain.Entities
{
    public class HotelAmenity : Entity
    {
        public string? Name { get; set; }
        public HotelAmenityCategoryEnum Category { get; set; }

        public virtual ICollection<HotelAmenityLink> HotelLinks { get; set; }
    }
}
