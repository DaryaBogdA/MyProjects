using Bogdanovich.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Bogdanovich.Domain.Entities
{
    public class User : Entity
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRoleEnum Role { get; set; } = UserRoleEnum.USER;

        public virtual UserProfile? Profile { get; set; }
        public virtual ICollection<Trip> Trips { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
