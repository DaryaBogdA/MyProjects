namespace Bogdanovich.Domain.Entities
{
    public class RoomAmenity : Entity
    {
        public string? Name { get; set; }
        public string? PhotoUrl { get; set; }

        public virtual ICollection<RoomTypeAmenityLink> RoomTypeLinks { get; set; }
    }
}
