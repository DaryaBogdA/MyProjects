using Bogdanovich.Domain.Enums;

namespace Bogdanovich.Domain.Entities
{
    public class TripItem : Entity
    {
        public int TripId { get; set; }
        public BaseEntityTypeEnum ItemType { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; } = 1;

        public virtual Trip? Trip { get; set; }
    }
}
