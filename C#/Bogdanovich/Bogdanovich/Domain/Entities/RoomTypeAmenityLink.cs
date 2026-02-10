namespace Bogdanovich.Domain.Entities
{
    public class RoomTypeAmenityLink
    {
        public int RoomTypeId { get; set; }
        public int AmenityId { get; set; }

        public virtual RoomType? RoomType { get; set; }
        public virtual RoomAmenity? Amenity { get; set; }
    }
}
