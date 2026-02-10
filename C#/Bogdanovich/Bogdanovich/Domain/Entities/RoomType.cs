namespace Bogdanovich.Domain.Entities
{
    public class RoomType : Entity
    {
        public int HotelId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal PriceNight { get; set; }
        public int MaxOccupancy { get; set; } = 2;
        public int? SizeSqm { get; set; }
        public bool HasBalcony { get; set; } = false;
        public bool SmokingAllowed { get; set; } = false;
        public bool PetFriendly { get; set; } = false;

        public virtual Hotel? Hotel { get; set; }
        public virtual ICollection<RoomTypeAmenityLink> AmenityLinks { get; set; }
        public virtual ICollection<RoomInventory> RoomInventories { get; set; }
    }
}
