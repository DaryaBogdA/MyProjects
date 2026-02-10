using Bogdanovich.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Bogdanovich.Domain.Entities
{
    public class Review : Entity
    {
        public int UserId { get; set; }
        public BaseEntityTypeEnum EntityType { get; set; }
        public int EntityId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public string? Comment { get; set; }

        public virtual User? User { get; set; }
    }
}
