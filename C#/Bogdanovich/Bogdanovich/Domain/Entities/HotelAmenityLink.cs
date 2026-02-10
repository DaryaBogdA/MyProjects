namespace Bogdanovich.Domain.Entities
{
    /// <summary>
    /// Many-to-many link between Hotel and HotelAmenity (table hotel_to_amenities).
    /// Composite PK: (HotelId, AmenityId) — no separate Id.
    /// </summary>
    public class HotelAmenityLink
    {
        public int HotelId { get; set; }
        public int AmenityId { get; set; }
        public bool IsPaid { get; set; } = false;
        public decimal? Price { get; set; }

        public virtual Hotel? Hotel { get; set; }
        public virtual HotelAmenity? Amenity { get; set; }
    }
}
