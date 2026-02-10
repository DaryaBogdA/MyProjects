using System.ComponentModel.DataAnnotations;

namespace Bogdanovich.Domain.Entities
{
    public class Hotel : Entity
    {
        public int CityId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? PhotoUrl { get; set; }
        [Range(1, 5)]
        public int Stars { get; set; }
        public string? ContactPhone { get; set; }

        public virtual City? City { get; set; }
        public virtual ICollection<HotelAmenityLink> AmenityLinks { get; set; }
        public virtual ICollection<RoomType> RoomTypes { get; set; }
        public virtual ICollection<RoomInventory> RoomInventories { get; set; }
    }
}
