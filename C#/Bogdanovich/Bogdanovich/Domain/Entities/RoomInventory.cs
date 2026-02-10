using Bogdanovich.Domain.Enums;

namespace Bogdanovich.Domain.Entities
{
    public class RoomInventory : Entity
    {
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public string? RoomNumber { get; set; }
        public int? Floor { get; set; }
        public RoomStatusEnum Status { get; set; } = RoomStatusEnum.AVAILABLE;

        public virtual Hotel? Hotel { get; set; }
        public virtual RoomType? RoomType { get; set; }
    }
}
